namespace FizzCode.DbTools.DataDefinition.Generic1
{
    using FizzCode.DbTools.Configuration;

    public static partial class Generic1Columns
    {
        public static SqlColumn AddNVarChar(int length, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GenericSqlType1.NVarChar,
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
            sqlColumn.Types.Add(GenericVersion.Generic1, sqlType);

            return sqlColumn;
        }

        public static SqlColumn AddInt32(bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = GenericSqlType1.Int32,
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