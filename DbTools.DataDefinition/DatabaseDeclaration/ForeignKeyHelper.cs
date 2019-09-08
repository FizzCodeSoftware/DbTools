namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;

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

            var fk = new ForeignKeyRegistrationToTableWithPrimaryKeyExistingColumn(singleFkColumn, referredTableNameWithSchema, fkName);

            singleFkColumn.Table.Properties.Add(fk);

            return singleFkColumn;
        }

        public static SqlTable SetForeignKeyTo(this SqlTable table, string referredTableName, List<ForeignKeyGroup> map, string fkName = null)
        {
            var referredTableNameWithSchema = new SchemaAndTableName(referredTableName);

            var fk = new ForeignKeyRegistrationToReferredTableExistingColumns(table, referredTableNameWithSchema, fkName, map);

            table.Properties.Add(fk);

            return table;
        }

        public static SqlTable AddForeignKey(this SqlTable table, string referredTableName, string singleFlColumnName, bool isNullable = false, string fkName = null)
        {
            var referredTableNameWithSchema = new SchemaAndTableName(referredTableName);

            var fk = new ForeignKeyRegistrationToTableWithPrimaryKeySingleColumn(table, referredTableNameWithSchema, singleFlColumnName, isNullable, fkName);

            table.Properties.Add(fk);

            return table;
        }

        public static SqlTable AddForeignKey(this SqlTable table, string referredTableName, List<ForeignKeyGroup> map, bool isNullable = false, string fkName = null)
        {
            var referredTableNameWithSchema = new SchemaAndTableName(referredTableName);

            var fk = new ForeignKeyRegistrationToReferredTable(table, referredTableNameWithSchema, isNullable, fkName, map);
            table.Properties.Add(fk);

            return table;
        }

        public static SqlTable AddForeignKey(this SqlTable table, string nameOfReferredTableWithPrimaryKey, bool isNullable = false, string prefix = null, string fkName = null)
        {
            var referredTableNameWithSchema = new SchemaAndTableName(nameOfReferredTableWithPrimaryKey);

            var fk = new ForeignKeyRegistrationToTableWithPrimaryKey(table, referredTableNameWithSchema, isNullable, prefix, fkName);
            table.Properties.Add(fk);

            return table;
        }
    }
}