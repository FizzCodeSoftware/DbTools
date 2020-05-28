namespace FizzCode.DbTools.DataDefinition.MsSql2016
{
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;

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