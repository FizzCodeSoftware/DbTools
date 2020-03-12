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

    // Other / all cases
    // New PK, Deleted PK, Renamed PK
    // New FK, Deleted FK, Renamed FK
    // FK NoCheck change - SqlEngineVersionSpecificProperty change
    // New Index, Deleted Index, Renamed Index

    // New TableDescription, Deleted TableDescription
    // New ColumnDescription, Deleted ColumnDescription

    // New DefaultValue, Deleted DefaultValue
    // New Identity, Deleted Identity

    // (not implemented) Trigger

    // SuspectedTableRename - if remove/add table but the internal schema is exactly the asame, suspect table rename
}
