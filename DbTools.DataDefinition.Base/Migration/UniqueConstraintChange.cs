namespace FizzCode.DbTools.DataDefinition.Base.Migration
{
    using System.Text;

    public class UniqueConstraintChange : UniqueConstraintMigration
    {
        public UniqueConstraint NewUniqueConstraint { get; set; }

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
}
