namespace FizzCode.DbTools.DataDefinition
{
    internal class ForeignKeyRegistrationToTableWithUniqueKeyExistingColumn : ForeignKeyRegistrationBase
    {
        public SqlColumn SingleFkColumn { get; set; }

        public ForeignKeyRegistrationToTableWithUniqueKeyExistingColumn(SqlColumn singleFkColumn, SchemaAndTableName referredTableName, string fkName)
            : base(singleFkColumn.Table, referredTableName, fkName)
        {
            SingleFkColumn = singleFkColumn;
        }
    }
}
