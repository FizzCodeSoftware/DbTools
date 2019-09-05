namespace FizzCode.DbTools.DataDefinition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class ForeignKeyHelper
    {
        /// <summary>
        /// Creates a set of columns (one or more, depeding on the PK columns) which will be an FK pointing to the PK of <paramref name="lazyReferredTable">.
        /// </summary>
        /// <param name="table">The table the column will be added to.</param>
        /// <param name="lazyReferredTable">The PK table.</param>
        /// <param name="columnNamePrefix">Column prefix used with naming strategy.</param>
        /// <param name="isNullable">New FK columns will be nullable, ff set to true.</param>
        /// <returns>The list of newly created column declaration.</returns>
        public static List<SqlColumnDeclaration> AddForeignKey(this SqlTableDeclaration table, LazySqlTable lazyReferredTable, string columnNamePrefix = null, bool isNullable = false)
        {
            SqlTable referredTable;
            try
            {
                try
                {
                    referredTable = lazyReferredTable.SqlTable;
                }
                catch (InvalidOperationException ex)
                {
                    throw new LazySqlTableSuspectedCircularReferenceException(ex);
                }
            }
            catch (LazySqlTableSuspectedCircularReferenceException)
            {
                var column = new SqlColumnLazyForeignKey(table, lazyReferredTable, columnNamePrefix, isNullable);
                var tempColumnName = table.LazyForeignKeys().Count.ToString();
                column.Name = tempColumnName;
                table.Columns.Add(column.Name, column);
                return new List<SqlColumnDeclaration>() { column };
            }

            var pk = referredTable.Properties.OfType<PrimaryKey>().First();

            if (pk == null || pk.SqlColumns.Count == 0)
                throw new ArgumentException("Referred table must have a Primary Key.", nameof(referredTable));

            var columns = new List<SqlColumnDeclaration>();

            foreach (var pkColumn in pk.SqlColumns)
            {
                // HACK circular reference - DatabaseDeclaration is not set yet.
                if (table.DatabaseDeclaration == null)
                    table.SetLazyProperties(table.SchemaAndTableName, lazyReferredTable.DatabaseDeclaration);

                var column = table.AddForeignKey(null, (SqlColumnDeclaration)pkColumn.SqlColumn, columnNamePrefix);
                column.IsNullable = isNullable;
                columns.Add(column);
            }

            return columns;
        }

        /// <summary>
        /// Sets an existing column as an FK, pointing to the PK of <paramref name="lazyReferredTable"/>.
        /// </summary>
        /// <param name="column"></param>
        /// <param name="lazyReferredTable"></param>
        /// <param name="isNullable">New FK columns will be nullable, ff set to true.</param>
        /// <returns>The original <paramref name="column"/>.</returns>
        public static SqlColumnDeclaration SetForeignKeyTo(this SqlColumnDeclaration column, LazySqlTable lazyReferredTable, bool isNullable = false)
        {
            SqlTable referredTable;
            try
            {
                try
                {
                    referredTable = lazyReferredTable.SqlTable;
                }
                catch (InvalidOperationException ex)
                {
                    throw new LazySqlTableSuspectedCircularReferenceException(ex);
                }
            }
            catch (LazySqlTableSuspectedCircularReferenceException)
            {
                var lazyColumn = new SqlColumnLazyForeignKeyFromSet(column, lazyReferredTable, isNullable);
                column.CopyTo(lazyColumn);
                
                // replace the already existing column
                column.Table.Columns[column.Name] = lazyColumn;

                return lazyColumn;
            }

            var pk = referredTable.Properties.OfType<PrimaryKey>().First();

            if (pk == null || pk.SqlColumns.Count == 0)
                throw new ArgumentException("Referred table must have a Primary Key.", nameof(referredTable));

            var fk = new ForeignKey(column.Table, pk, null);
            column.Table.Properties.Add(fk);

            fk.ForeignKeyColumns.Add(new ForeignKeyColumnMap(column, pk.SqlColumns.Single().SqlColumn));

            var fkNaming = column.Table.DatabaseDeclaration?.NamingStrategies.GetNamingStrategy<IForeignKeyNamingStrategy>();
            fkNaming?.SetFKName(fk);

            if (fk.Name == null)
                column.Table.DelayedNamingTasks.Add(new DelayedNamingForeignKey(fk));

            return column;
        }

        /// <summary>
        /// Creates a new column which will be an FK pointing to <paramref name="pkColumn">.
        /// The FK column properties (Type, IsNullable, Length, Precision) will be a copy of the <paramref name="pkColumn"/>.
        /// </summary>
        /// <param name="table">The table the column will be added to.</param>
        /// <param name="columnName">Name of the FK column to create. If null, the naming strategy will determine the name of the newwly created column.</param>
        /// <param name="pkColumn">The PK column the <paramref name="columnName"/> will point to.</param>
        /// <param name="columnNamePrefix">Column prefix used with naming strategy.</param>
        /// <param name="existingForeignKey">If the column is a part of a composite (multiple column) foreign key, provide the defined FK.</param>
        /// <returns>The newly created column declaration.</returns>
        public static SqlColumnDeclaration AddForeignKey(this SqlTableDeclaration table, string columnName, SqlColumn pkColumn, string columnNamePrefix = null, ForeignKey existingForeignKey = null)
        {
            var fkColumn = pkColumn.CopyTo(new SqlColumnDeclaration());
            fkColumn.Table = table;

            ForeignKey fk;
            var pk = pkColumn.Table.Properties.OfType<PrimaryKey>().First();

            if (existingForeignKey != null)
            {
                fk = existingForeignKey;
            }
            else
            {
                fk = new ForeignKey(table, pk, null);
                table.Properties.Add(fk);
            }

            var fkNaming = table.DatabaseDeclaration?.NamingStrategies.GetNamingStrategy<IForeignKeyNamingStrategy>();

            fk.ForeignKeyColumns.Add(new ForeignKeyColumnMap(fkColumn, pkColumn));

            if (columnName == null)
                fkNaming?.SetFKColumnsNames(fk, columnNamePrefix);
            else
                fkColumn.Name = columnName;

            if (fkColumn.Name == null)
                table.DelayedNamingTasks.Add(new DelayedNamingForeignKeyColumn(fk));

            fkNaming?.SetFKName(fk);
            if (fk.Name == null)
                table.DelayedNamingTasks.Add(new DelayedNamingForeignKey(fk));

            // TODO possible exception on same key
            // TODO test with multi FK
            table.Columns.Add(fkColumn.Name, fkColumn);

            return fkColumn;
        }

        public static List<SqlColumn> AddForeignKey(this SqlTableDeclaration table, params ForeignKeyGroup[] foreignKeyGroups)
        {
            var columns = new List<SqlColumn>();

            var pk = foreignKeyGroups[0].PKColumn.Table.Properties.OfType<PrimaryKey>().First();
            // TODO check primary key
            // TODO check all foreignKeyGroups points to the same PK
            // TODO check number of columns are the same

            var fk = new ForeignKey(table, pk, null);

            // HACK circular reference - DatabaseDeclaration is not set yet.
            if (table.DatabaseDeclaration == null)
                table.SetLazyProperties(table.SchemaAndTableName, foreignKeyGroups[0].PKColumn.Table.DatabaseDeclaration);

            var fkNaming = table.DatabaseDeclaration.NamingStrategies.GetNamingStrategy<IForeignKeyNamingStrategy>();
            fkNaming.SetFKName(fk);
            table.Properties.Add(fk);

            foreach (var foreignKeyGroup in foreignKeyGroups)
            {
                var column = AddForeignKey(table, foreignKeyGroup.ColumnName, foreignKeyGroup.PKColumn, null, fk);
                columns.Add(column);
            }

            return columns;
        }
    }
}
