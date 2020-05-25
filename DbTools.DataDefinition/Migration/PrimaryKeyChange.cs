namespace FizzCode.DbTools.DataDefinition.Migration
{
    using System.Text;

    public class PrimaryKeyChange : PrimaryKeyMigration
    {
        public PrimaryKey NewPrimaryKey { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("PC: New: ");
            sb.AppendLine(NewPrimaryKey.ToString());
            sb.AppendLine(", Orig: ");
            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
    }
}
