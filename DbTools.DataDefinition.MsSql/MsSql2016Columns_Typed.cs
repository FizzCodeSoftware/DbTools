namespace FizzCode.DbTools.DataDefinition.MsSql2016
{
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;

    public static class ForeignKeyHelper
    {
        /// <summary>
        /// Sets an existing column as an FK, pointing to the <paramref name="referredColumnName"/> of <paramref name="referredTableName"/>, with the Nocheck property.
        /// Note <paramref name="referredColumnName"/> has to be a unique key.
        /// </summary>
        /// <param name="singleFkColumn">The existing column to set as FK.</param>
        /// <param name="referredTableName">The name of the referred table.</param>
        /// <param name="referredColumnName">The name of the referred column.</param>
        /// <param name="fkName"></param>
        /// <returns>The original <paramref name="singleFkColumn"/>.</returns>
        public static SqlColumn SetForeignKeyToColumnNoCheck(this SqlColumn singleFkColumn, string referredTableName, string referredColumnName, string fkName = null)
        {
            var property = new SqlEngineVersionSpecificProperty(MsSqlVersion.MsSql2016, "Nocheck", "true");
            return singleFkColumn.SetForeignKeyToColumn(referredTableName, referredColumnName, property, fkName);
        }
    }

    public static partial class MsSql2016Columns
    {
        public static SqlColumn AddNVarChar(int length, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = MsSqlType2016.NVarChar,
                Length = length,
                IsNullable = isNullable
            };

            return Add(sqlType);
        }

        private static SqlColumn Add(SqlType sqlType)
        {
            var sqlColumn = new SqlColumn
            {
                Table = new SqlTable() // dummy Sql Table
            };
            sqlColumn.Types.Add(MsSqlVersion.MsSql2016, sqlType);

            return sqlColumn;
        }

        public static SqlColumn AddInt(bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = MsSqlType2016.Int,
                IsNullable = isNullable
            };

            return Add(sqlType);
        }

        public static SqlColumn SetForeignKeyTo(string referredTableName, string fkName = null)
        {
            var referredTableNameWithSchema = new SchemaAndTableName(referredTableName);

            var singleFkColumn = new SqlColumn
            {
                Table = new SqlTable()
            };

            var fk = new ForeignKeyRegistrationToTableWithUniqueKeyExistingColumn(singleFkColumn, referredTableNameWithSchema, null, fkName);

            singleFkColumn.Table.Properties.Add(fk);

            return singleFkColumn;
        }
    }
}