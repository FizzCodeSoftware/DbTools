namespace FizzCode.DbTools.DataDefinition.Base
{
    public abstract class ForeignKeyRegistrationBase : ForeignKey
    {
        public SchemaAndTableName ReferredTableName { get; }

        protected ForeignKeyRegistrationBase(SqlTable table, SchemaAndTableName referredTableName, string name)
            : base(table, null, name)
        {
            Name = name;
            ReferredTableName = referredTableName;
        }
    }
}
