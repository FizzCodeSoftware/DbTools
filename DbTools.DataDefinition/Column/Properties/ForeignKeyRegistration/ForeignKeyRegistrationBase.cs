namespace FizzCode.DbTools.DataDefinition
{
    internal abstract class ForeignKeyRegistrationBase : SqlTableProperty
    {
        public string Name { get; set; }
        public SchemaAndTableName ReferredTableName { get; }

        protected ForeignKeyRegistrationBase(SqlTable table, SchemaAndTableName referredTableName, string name) : base(table)
        {
            Name = name;
            ReferredTableName = referredTableName;
        }
    }
}
