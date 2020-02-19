namespace FizzCode.DbTools.DataDefinition
{
    internal class ForeignKeyRegistrationToTableWithUniqueKeySingleColumn : ForeignKeyRegistrationNonExsistingColumn
    {
        public string SingleFkColumnName { get; set; }

        public ForeignKeyRegistrationToTableWithUniqueKeySingleColumn(SqlTable table, SchemaAndTableName referredTableName, string singleFkColumnName, bool isNullable, string fkName)
            : base(table, referredTableName, isNullable, fkName)
        {
            SingleFkColumnName = singleFkColumnName;
        }
    }
}