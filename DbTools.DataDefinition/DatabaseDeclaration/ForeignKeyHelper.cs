namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    public static class ForeignKeyHelper
    {
        // TODO should throw if PK is multi column

        /// <summary>
        /// Sets an existing column as an FK, pointing to the PK of <paramref name="referredTableName"/>.
        /// </summary>
        /// <param name="singleFkColumn"></param>
        /// <param name="referredTableName"></param>
        /// <param name="fkName"></param>
        /// <returns>The original <paramref name="singleFkColumn"/>.</returns>
        public static SqlColumn SetForeignKeyTo(this SqlColumn singleFkColumn, string referredTableName, string fkName = null)
        {
            var referredTableNameWithSchema = new SchemaAndTableName(referredTableName);

            var fk = new ForeignKeyRegistrationToTableWithUniqueKeyExistingColumn(singleFkColumn, referredTableNameWithSchema, fkName);

            singleFkColumn.Table.Properties.Add(fk);

            return singleFkColumn;
        }

        public static SqlColumn SetForeignKeyTo(this SqlColumn singleFkColumn, string referredTableName, IEnumerable<SqlEngineVersionSpecificProperty> properties, string fkName = null)
        {
            var referredTableNameWithSchema = new SchemaAndTableName(referredTableName);

            var fk = new ForeignKeyRegistrationToTableWithUniqueKeyExistingColumn(singleFkColumn, referredTableNameWithSchema, fkName);
            fk.SqlEngineVersionSpecificProperties.Add(properties);

            singleFkColumn.Table.Properties.Add(fk);

            return singleFkColumn;
        }

        public static SqlTable SetForeignKeyTo(this SqlTable table, string referredTableName, List<ColumnReference> map, string fkName = null)
        {
            var referredTableNameWithSchema = new SchemaAndTableName(referredTableName);

            var fk = new ForeignKeyRegistrationToReferredTableExistingColumns(table, referredTableNameWithSchema, fkName, map);

            table.Properties.Add(fk);

            return table;
        }

        public static SqlTable SetForeignKeyTo(this SqlTable table, string referredTableName, List<ColumnReference> map, IEnumerable<SqlEngineVersionSpecificProperty> properties, string fkName = null)
        {
            var referredTableNameWithSchema = new SchemaAndTableName(referredTableName);

            var fk = new ForeignKeyRegistrationToReferredTableExistingColumns(table, referredTableNameWithSchema, fkName, map);
            fk.SqlEngineVersionSpecificProperties.Add(properties);

            table.Properties.Add(fk);

            return table;
        }

        public static SqlTable AddForeignKey(this SqlTable table, string referredTableName, string singleFkColumnName, bool isNullable = false, string fkName = null)
        {
            var referredTableNameWithSchema = new SchemaAndTableName(referredTableName);

            var fk = new ForeignKeyRegistrationToTableWithUniqueKeySingleColumn(table, referredTableNameWithSchema, singleFkColumnName, isNullable, fkName);

            var placeHolderColumnName = $"*{referredTableNameWithSchema}.{singleFkColumnName}.{table.Columns.Count.ToString("D", CultureInfo.InvariantCulture)}";
            table.Columns.Add(new SqlColumnFKRegistration(placeHolderColumnName, fk));

            table.Properties.Add(fk);

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

            return table;
        }

        public static SqlTable AddForeignKey(this SqlTable table, string nameOfReferredTableWithUniqueKey, bool isNullable = false, string prefix = null, string fkName = null)
        {
            var referredTableNameWithSchema = new SchemaAndTableName(nameOfReferredTableWithUniqueKey);

            var fk = new ForeignKeyRegistrationToTableWithUniqueKey(table, referredTableNameWithSchema, isNullable, prefix, fkName);

            var placeHolderColumnName = $"*{referredTableNameWithSchema}.{table.Columns.Count.ToString("D", CultureInfo.InvariantCulture)}";
            table.Columns.Add(new SqlColumnFKRegistration(placeHolderColumnName, fk));

            table.Properties.Add(fk);
            return table;
        }
    }
}