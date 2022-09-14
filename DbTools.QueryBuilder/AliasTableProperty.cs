namespace FizzCode.DbTools.QueryBuilder
{
    using FizzCode.DbTools.DataDefinition;

    public class AliasTableProperty : SqlTableCustomProperty
    {
        public string Alias { get; set; }
    }

    public class AliasViewProperty : SqlViewCustomProperty
    {
        public string Alias { get; set; }
    }
}
