namespace FizzCode.DbTools.DataDefinition
{
    public class ForeignKeyRegistrationToTableWithPrimaryKeySingleColumn : ForeignKeyRegistrationBase
    {
        public string SingleFkColumnName { get; set; }
        public bool IsNullable { get; set; }

        public ForeignKeyRegistrationToTableWithPrimaryKeySingleColumn(SqlTable table, SchemaAndTableName referredTableName, string singleFkColumnName, bool isNullable, string fkName) : base(table, referredTableName, fkName)
        {
            SingleFkColumnName = singleFkColumnName;
            IsNullable = isNullable;
        }
    }
}
