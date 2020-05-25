namespace FizzCode.DbTools.DataDefinition.Migration
{
    using System.Text;

    public class IndexChange : IndexMigration
    {
        public Index NewIndex { get; set; }

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
}
