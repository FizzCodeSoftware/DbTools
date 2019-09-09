namespace FizzCode.DbTools.DataDefinition
{
    public abstract class ForeignKeyRegistrationBase : SqlTableProperty
    {
        public string Name { get; set; }
        public SchemaAndTableName ReferredTableName { get; }

        public ForeignKeyRegistrationBase(SqlTable table, SchemaAndTableName referredTableName, string name) : base(table)
        {
            Name = name;
            ReferredTableName = referredTableName;
        }
    }
}
