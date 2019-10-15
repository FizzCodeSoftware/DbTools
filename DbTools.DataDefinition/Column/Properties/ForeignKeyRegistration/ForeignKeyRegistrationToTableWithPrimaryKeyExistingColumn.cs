namespace FizzCode.DbTools.DataDefinition
{
    internal class ForeignKeyRegistrationToTableWithPrimaryKeyExistingColumn : ForeignKeyRegistrationBase
    {
        public SqlColumn SingleFkColumn { get; set; }

        public ForeignKeyRegistrationToTableWithPrimaryKeyExistingColumn(SqlColumn singleFkColumn, SchemaAndTableName referredTableName, string fkName)
            : base(singleFkColumn.Table, referredTableName, fkName)
        {
            SingleFkColumn = singleFkColumn;
        }
    }
}
