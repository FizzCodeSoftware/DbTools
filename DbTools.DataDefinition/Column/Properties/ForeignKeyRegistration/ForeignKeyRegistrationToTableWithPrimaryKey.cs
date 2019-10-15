namespace FizzCode.DbTools.DataDefinition
{
    internal class ForeignKeyRegistrationToTableWithPrimaryKey : ForeignKeyRegistrationBase
    {
        public bool IsNullable { get; set; }
        public string NamePrefix { get; set; }

        public ForeignKeyRegistrationToTableWithPrimaryKey(SqlTable table, SchemaAndTableName referredTableName, bool isNullable, string namePrefix, string fkName)
            : base(table, referredTableName, fkName)
        {
            IsNullable = isNullable;
            NamePrefix = namePrefix;
        }
    }
}
