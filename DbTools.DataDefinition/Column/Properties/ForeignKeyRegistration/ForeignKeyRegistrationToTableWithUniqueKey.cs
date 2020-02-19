namespace FizzCode.DbTools.DataDefinition
{
    internal class ForeignKeyRegistrationToTableWithUniqueKey : ForeignKeyRegistrationNonExsistingColumn
    {
        public string NamePrefix { get; set; }

        public ForeignKeyRegistrationToTableWithUniqueKey(SqlTable table, SchemaAndTableName referredTableName, bool isNullable, string namePrefix, string fkName)
            : base(table, referredTableName, isNullable, fkName)
        {
            NamePrefix = namePrefix;
        }
    }
}
