using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDefinition;
/// <summary>
/// Contains the <see cref="Description"/>, to document the given table.
/// </summary>
public class SqlTableDescription(SqlTable sqlTable, string description)
    : SqlTableOrViewPropertyBase<SqlTable>(sqlTable)
{
    public string Description { get; set; } = description;
}
