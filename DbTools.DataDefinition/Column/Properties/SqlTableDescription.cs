using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDefinition;
/// <summary>
/// Contains the <see cref="Description"/>, to document the given table.
/// </summary>
public class SqlTableDescription : SqlTableOrViewPropertyBase<SqlTable>
{
    public SqlTableDescription(SqlTable sqlTable, string description)
        : base(sqlTable)
    {
        Description = description;
    }

    public string Description { get; set; }
}
