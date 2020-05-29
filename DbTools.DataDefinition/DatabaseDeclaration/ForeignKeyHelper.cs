namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    public static class ForeignKeyHelper
    {
        /// <summary>
        /// Sets an existing column as an FK, pointing to the unique key of <paramref name="referredTableName"/>.
        /// </summary>
        /// <param name="singleFkColumn">The existing column to set as FK.</param>
        /// <param name="referredTableName">The name of the referred table.</param>
        /// <param name="fkName">The name of the FK. Auto naming will set name if not provided.</param>
        /// <returns>The original <paramref name="singleFkColumn"/>.</returns>
        public static SqlColumn SetForeignKeyToTable(this SqlColumn singleFkColumn, string referredTableName, string fkName = null)
        {
            var referredTableNameWithSchema = new SchemaAndTableName(referredTableName);
            var fk = new ForeignKeyRegistrationToTableWithUniqueKeyExistingColumn(singleFkColumn, referredTableNameWithSchema, null, fkName);
            singleFkColumn.Table.Properties.Add(fk);
            Prepare(singleFkColumn.Table);
            return singleFkColumn;
        }

        public static SqlColumn SetForeignKeyToColumn(this SqlColumn singleFkColumn, string referredTableName, string referredColumnName, string fkName = null)
        {
            var referredTableNameWithSchema = new SchemaAndTableName(referredTableName);
            var fk = new ForeignKeyRegistrationToTableWithUniqueKeyExistingColumn(singleFkColumn, referredTableNameWithSchema, referredColumnName, fkName);
            singleFkColumn.Table.Properties.Add(fk);
            Prepare(singleFkColumn.Table);
            return singleFkColumn;
        }

        /// <summary>
        /// Sets an existing column as an FK, pointing to the unique key of <paramref name="referredSchemaAndTableName"/>.
        /// </summary>
        /// <param name="singleFkColumn">The existing column to set as FK.</param>
        /// <param name="referredSchemaAndTableName">The referred table as <see cref="SchemaAndTableName"/>.</param>
        /// <param name="fkName">The name of the FK. Auto naming will set name if not provided.</param>
        /// <returns>The original <paramref name="singleFkColumn"/>.</returns>
        public static SqlColumn SetForeignKeyToTable(this SqlColumn singleFkColumn, SchemaAndTableName referredSchemaAndTableName, string fkName = null)
        {
            var fk = new ForeignKeyRegistrationToTableWithUniqueKeyExistingColumn(singleFkColumn, referredSchemaAndTableName, null, fkName);
            singleFkColumn.Table.Properties.Add(fk);
            Prepare(singleFkColumn.Table);
            return singleFkColumn;
        }

        /// <summary>
        /// Sets an existing column as an FK, pointing to the unique key of <paramref name="referredSchemaAndTableName"/>, providing <paramref name="properties"/>.
        /// </summary>
        /// <param name="singleFkColumn">The existing column to set as FK.</param>
        /// <param name="referredSchemaAndTableName">The referred table as <see cref="SchemaAndTableName"/>.</param>
        /// <param name="properties"></param>
        /// <param name="fkName">The name of the FK. Auto naming will set name if not provided.</param>
        /// <returns>The original <paramref name="singleFkColumn"/>.</returns>
        public static SqlColumn SetForeignKeyToTable(this SqlColumn singleFkColumn, SchemaAndTableName referredSchemaAndTableName, IEnumerable<SqlEngineVersionSpecificProperty> properties, string fkName = null)
        {
            var fk = new ForeignKeyRegistrationToTableWithUniqueKeyExistingColumn(singleFkColumn, referredSchemaAndTableName, null, fkName);
            fk.SqlEngineVersionSpecificProperties.Add(properties);
            singleFkColumn.Table.Properties.Add(fk);
            Prepare(singleFkColumn.Table);
            return singleFkColumn;
        }

        /// <summary>
        /// Sets an existing column as an FK, pointing to the <paramref name="referredColumnName"/> of <paramref name="referredTableName"/>, providing <paramref name="properties"/>.
        /// Note <paramref name="referredColumnName"/> has to be a unique key.
        /// </summary>
        /// <param name="singleFkColumn">The existing column to set as FK.</param>
        /// <param name="referredTableName">The name of the referred table.</param>
        /// <param name="referredColumnName">The name of the referred column.</param>
        /// <param name="properties"></param>
        /// <param name="fkName"></param>
        /// <returns>The original <paramref name="singleFkColumn"/>.</returns>
        public static SqlColumn SetForeignKeyToColumn(this SqlColumn singleFkColumn, string referredTableName, string referredColumnName, IEnumerable<SqlEngineVersionSpecificProperty> properties, string fkName = null)
        {
            var referredTableNameWithSchema = new SchemaAndTableName(referredTableName);

            var fk = new ForeignKeyRegistrationToTableWithUniqueKeyExistingColumn(singleFkColumn, referredTableNameWithSchema, referredColumnName, fkName);
            fk.SqlEngineVersionSpecificProperties.Add(properties);

            singleFkColumn.Table.Properties.Add(fk);

            Prepare(singleFkColumn.Table);

            return singleFkColumn;
        }

        public static SqlColumn SetForeignKeyToTable(this SqlColumn singleFkColumn, string referredTableName, IEnumerable<SqlEngineVersionSpecificProperty> properties, string fkName = null)
        {
            var referredTableNameWithSchema = new SchemaAndTableName(referredTableName);

            var fk = new ForeignKeyRegistrationToTableWithUniqueKeyExistingColumn(singleFkColumn, referredTableNameWithSchema, null, fkName);
            fk.SqlEngineVersionSpecificProperties.Add(properties);

            singleFkColumn.Table.Properties.Add(fk);

            Prepare(singleFkColumn.Table);

            return singleFkColumn;
        }

        public static SqlTable SetForeignKeyTo(this SqlTable table, string referredTableName, ColumnReference[] columnReferences)
        {
            return SetForeignKeyTo(table, referredTableName, (string)null, columnReferences);
        }

        public static SqlTable SetForeignKeyTo(this SqlTable table, string referredTableName, string fkName, ColumnReference[] columnReferences)
        {
            return SetForeignKeyTo(table, referredTableName, fkName, null, columnReferences);
        }

        public static SqlTable SetForeignKeyTo(this SqlTable table, string referredTableName, IEnumerable<SqlEngineVersionSpecificProperty> properties, ColumnReference[] columnReferences)
        {
            return SetForeignKeyTo(table, referredTableName, null, properties, columnReferences);
        }

        public static SqlTable SetForeignKeyTo(this SqlTable table, string referredTableName, string fkName, IEnumerable<SqlEngineVersionSpecificProperty> properties, ColumnReference[] columnReferences)
        {
            var map = new List<ColumnReference>(columnReferences);
            var referredTableNameWithSchema = new SchemaAndTableName(referredTableName);
            var fk = new ForeignKeyRegistrationToReferredTableExistingColumns(table, referredTableNameWithSchema, fkName, map);
            if (properties != null)
                fk.SqlEngineVersionSpecificProperties.Add(properties);

            table.Properties.Add(fk);
            Prepare(table);
            return table;
        }

        public static SqlTable AddForeignKey(this SqlTable table, string referredTableName, string singleFkColumnName, bool isNullable = false, string fkName = null)
        {
            var referredTableNameWithSchema = new SchemaAndTableName(referredTableName);

            var fk = new ForeignKeyRegistrationToTableWithUniqueKeySingleColumn(table, referredTableNameWithSchema, singleFkColumnName, isNullable, fkName);

            var placeHolderColumnName = $"*{referredTableNameWithSchema}.{singleFkColumnName}.{table.Columns.Count.ToString("D", CultureInfo.InvariantCulture)}";
            table.Columns.Add(new SqlColumnFKRegistration(placeHolderColumnName, fk));

            table.Properties.Add(fk);

            Prepare(table);

            return table;
        }

        public static SqlTable AddForeignKey(this SqlTable table, string referredTableName, List<ColumnReference> map, bool isNullable = false, string fkName = null)
        {
            var referredTableNameWithSchema = new SchemaAndTableName(referredTableName);

            var fk = new ForeignKeyRegistrationToReferredTable(table, referredTableNameWithSchema, isNullable, fkName, map);
            table.Properties.Add(fk);

            var mapColumnNames = string.Join("_", map.Select(m => m.ColumnName).ToList());

            var placeHolderColumnName = $"*{referredTableNameWithSchema}.{mapColumnNames}.{table.Columns.Count.ToString("D", CultureInfo.InvariantCulture)}";
            table.Columns.Add(new SqlColumnFKRegistration(placeHolderColumnName, fk));

            Prepare(table);

            return table;
        }

        public static SqlTable AddForeignKey(this SqlTable table, string referredTableName, bool isNullable = false, string prefix = null, string fkName = null)
        {
            var referredTableNameWithSchema = new SchemaAndTableName(referredTableName);

            var fk = new ForeignKeyRegistrationToTableWithUniqueKey(table, referredTableNameWithSchema, isNullable, prefix, fkName);

            var placeHolderColumnName = $"*{referredTableNameWithSchema}.{table.Columns.Count.ToString("D", CultureInfo.InvariantCulture)}";
            table.Columns.Add(new SqlColumnFKRegistration(placeHolderColumnName, fk));

            table.Properties.Add(fk);

            Prepare(table);

            return table;
        }

        public static void Prepare(SqlTable table)
        {
            if (table.DatabaseDefinition is DatabaseDeclaration dd)
            {
                dd.CreateRegisteredForeignKeys(table);
                dd.AddAutoNaming(new List<SqlTable>() { table });
                CircularFKDetector.DectectCircularFKs(new List<SqlTable>() { table });
            }
        }
    }
}