using System.Text;

namespace FizzCode.DbTools.DataDefinition.Base;
public class ForeignKey(SqlTable table, SqlTable? referredTable, string? name)
    : SqlTableOrViewPropertyBase<SqlTable>(table)
{
    public string? Name { get; set; } = name;

    public List<ForeignKeyColumnMap> ForeignKeyColumns { get; set; } = [];

    public SqlTable? ReferredTable { get; } = referredTable;

    public SqlTable SqlTable { get => SqlTableOrView!; }

    public override string ToString()
    {
        var sb = new StringBuilder();

        var isFirst = true;
        foreach (var fkColumn in ForeignKeyColumns)
        {
            if (!isFirst)
                sb.Append(", ");
            else
                isFirst = false;

            sb.Append(fkColumn.ForeignKeyColumn.Table.SchemaAndTableNameSafe)
                .Append('.')
                .Append(fkColumn.ForeignKeyColumn.Name)
                .Append(" -> ")
                .Append(fkColumn.ReferredColumn.Table.SchemaAndTableNameSafe)
                .Append('.')
                .Append(fkColumn.ReferredColumn.Name);
        }

        return sb.ToString();
    }
}
