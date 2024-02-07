using System.Text;

namespace FizzCode.DbTools.DataDefinition.Base.Migration;
public class IndexChange : IndexMigration
{
    public required Index NewIndex { get; init; }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine("IC: New: ");
        sb.AppendLine(NewIndex.ToString());
        sb.AppendLine(", Orig: ");
        sb.AppendLine(base.ToString());

        return sb.ToString();
    }
}
