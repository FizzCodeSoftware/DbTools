using System.Linq;
using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.QueryBuilder;
public class QueryColumn
{
    public QueryColumn()
    {
        Value = string.Empty;
    }

    public QueryColumn(QueryColumn column, string @as)
    {
        Value = column.Value;
        Alias = column.Alias;
        As = @as;
        IsDbColumn = column.IsDbColumn;
    }

    public QueryColumn(string value, string alias)
    {
        Value = value;
        As = alias;
    }

    public string Value { get; init; }

    /// <summary>
    /// The name of the column, if different from Value (ex. ou.OrgUnitId AS 'MyId').
    /// </summary>
    public string? As { get; set; }

    /// <summary>
    /// The alias for the table.
    /// </summary>
    public string? Alias { get; set; }

    public bool IsDbColumn { get; set; }

    public static implicit operator QueryColumn(SqlColumn column)
    {
        var queryColumn = new QueryColumn
        {
            Value = column.Name!,
            IsDbColumn = true,
        };

        var aliasTableProperty = column.Table.Properties.OfType<AliasTableProperty>().FirstOrDefault();
        if (aliasTableProperty != null)
            queryColumn.Alias = aliasTableProperty.Alias;

        return queryColumn;
    }

    public static implicit operator QueryColumn(SqlViewColumn column)
    {
        var queryColumn = new QueryColumn
        {
            Value = column.Name!,
            IsDbColumn = true,
        };

        var aliasTableProperty = column.View.Properties.OfType<AliasViewProperty>().FirstOrDefault();
        if (aliasTableProperty != null)
            queryColumn.Alias = aliasTableProperty.Alias;

        return queryColumn;
    }

    public override string ToString()
    {
        var sb = new System.Text.StringBuilder();
        sb.Append(Value);
        if (As != null)
        {
            sb.Append(" AS ");
            sb.Append(As);
        }
        if (IsDbColumn)
        {
            sb.Append(" (DbColumn)");
        }

        return sb.ToString();
    }
}
