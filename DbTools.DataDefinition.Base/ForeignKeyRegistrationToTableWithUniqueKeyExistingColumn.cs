namespace FizzCode.DbTools.DataDefinition.Base
{
    public class ForeignKeyRegistrationToTableWithUniqueKeyExistingColumn : ForeignKeyRegistrationBase
    {
        public SqlColumn SingleFkColumn { get; set; }
        public string SingleReferredColumnName { get; set; }

        public ForeignKeyRegistrationToTableWithUniqueKeyExistingColumn(SqlColumn singleFkColumn, SchemaAndTableName referredTableName, string singleReferredColumnName, string fkName)
            : base((SqlTable)singleFkColumn.Table, referredTableName, fkName)
        {
            SingleFkColumn = singleFkColumn;
            SingleReferredColumnName = singleReferredColumnName;
        }
    }
}
