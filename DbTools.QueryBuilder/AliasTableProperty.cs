using FizzCode.DbTools.DataDefinition;

namespace FizzCode.DbTools.QueryBuilder;
public class AliasTableProperty(string alias) : SqlTableCustomProperty
{
    public string Alias { get; set; } = alias;
}

public class AliasViewProperty(string alias) : SqlViewCustomProperty
{
    public string Alias { get; set; } = alias;
}
