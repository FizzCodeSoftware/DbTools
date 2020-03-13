namespace FizzCode.DbTools.DataDefinition.Migration
{
    using System.Text;

    public class ColumnChange : ColumnMigration
    {
        public SqlColumn NewNameAndType { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("New column: ");
            sb.AppendLine(NewNameAndType.ToString());
            sb.AppendLine("Original column: ");
            sb.AppendLine(base.ToString());

            return sb.ToString();
        }
    }
}
