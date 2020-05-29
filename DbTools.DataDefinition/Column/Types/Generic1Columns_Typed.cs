namespace FizzCode.DbTools.DataDefinition.Generic1
{
    using FizzCode.DbTools.Configuration;

    //public class IndexRegistration

    public static partial class Generic1Columns
    {
        public static Index AddIndex(params string[] columnNames)
        {
            var table = new SqlTable(); // dummy SqlTable
            var index = new Index(table, null);

            foreach (var columnName in columnNames)
                index.SqlColumns.Add(new ColumnAndOrderRegistration(columnName, AscDesc.Asc));

            return index;
        }

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
                Table = new SqlTable() // dummy SqlTable
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