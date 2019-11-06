namespace FizzCode.DbTools.DataDefinition
{
    internal class ForeignKeyRegistrationToTableWithPrimaryKey : ForeignKeyRegistrationNonExsistingColumn
    {
        public string NamePrefix { get; set; }

        public ForeignKeyRegistrationToTableWithPrimaryKey(SqlTable table, SchemaAndTableName referredTableName, bool isNullable, string namePrefix, string fkName)
            : base(table, referredTableName, isNullable, fkName)
        {
            NamePrefix = namePrefix;
        }
    }
}
