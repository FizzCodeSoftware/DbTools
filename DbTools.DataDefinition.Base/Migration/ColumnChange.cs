using System.Text;

namespace FizzCode.DbTools.DataDefinition.Base.Migration;
public class ColumnChange : ColumnMigration
{
    public required SqlColumn SqlColumnChanged { get; init; }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine("CC: New column: ");
        sb.AppendLine(SqlColumnChanged?.ToString());
        sb.AppendLine(", Original column: ");
        sb.AppendLine(base.ToString());

        return sb.ToString();
    }

    private List<SqlColumnPropertyMigration>? _sqlColumnPropertyMigrations;

    public List<SqlColumnPropertyMigration> SqlColumnPropertyMigrations => _sqlColumnPropertyMigrations ??= [];
}
