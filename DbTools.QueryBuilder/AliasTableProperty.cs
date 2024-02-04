using FizzCode.DbTools.DataDefinition;

namespace FizzCode.DbTools.QueryBuilder;
public class AliasTableProperty : SqlTableCustomProperty
{
    public string Alias { get; set; }
}

public class AliasViewProperty : SqlViewCustomProperty
{
    public string Alias { get; set; }
}
