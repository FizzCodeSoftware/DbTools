using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDefinition;
/// <summary>
/// Contains the <see cref="Description"/>, to document the given column.
/// </summary>
public class SqlColumnDescription(SqlColumnBase sqlColumn, string description) : SqlColumnProperty(sqlColumn)
{
    public string Description { get; set; } = description;
}