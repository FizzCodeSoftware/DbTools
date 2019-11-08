namespace FizzCode.DbTools.DataDefinition
{
    internal class ForeignKeyRegistrationToTableWithPrimaryKeySingleColumn : ForeignKeyRegistrationNonExsistingColumn
    {
        public string SingleFkColumnName { get; set; }

        public ForeignKeyRegistrationToTableWithPrimaryKeySingleColumn(SqlTable table, SchemaAndTableName referredTableName, string singleFkColumnName, bool isNullable, string fkName)
            : base(table, referredTableName, isNullable, fkName)
        {
            SingleFkColumnName = singleFkColumnName;
        }
    }
}