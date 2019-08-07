namespace FizzCode.DbTools.DataDefinition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class ForeignKeyHelper
    {
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
                    table.SetLazyProperties(table.Name, lazyReferredTable.DatabaseDeclaration);

                var column = table.AddForeignKey(null, (SqlColumnDeclaration)pkColumn.SqlColumn, columnNamePrefix);
                column.IsNullable = isNullable;
                columns.Add(column);
            }

            return columns;
        }

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
                table.SetLazyProperties(table.Name, foreignKeyGroups[0].PKColumn.Table.DatabaseDeclaration);

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
