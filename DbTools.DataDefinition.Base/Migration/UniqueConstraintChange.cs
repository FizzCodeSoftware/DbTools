using System.Text;

namespace FizzCode.DbTools.DataDefinition.Base.Migration;
public class UniqueConstraintChange : UniqueConstraintMigration
{
    public required UniqueConstraint NewUniqueConstraint { get; init; }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine("UC: New: ");
        sb.AppendLine(NewUniqueConstraint.ToString());
        sb.AppendLine(", Orig: ");
        sb.AppendLine(base.ToString());

        return sb.ToString();
    }
}
