namespace FizzCode.DbTools.DataDefinition.Migration
{
    public abstract class ColumnMigration : IMigration
    {
        public SqlColumn SqlColumn { get; set; }

        public override string ToString()
        {
            return SqlColumn.ToString();
        }
    }

    // Other / all cases
    // New PK, Deleted OK, Renamed PK
    // New FK, Deleted FK, Renamed FK
    // New Index, Deleted Index, Renamed Index

    // New TableDescription, Deleted TableDescription
    // New ColumnDescription, Deleted ColumnDescription

    // New DefaultValue, Deleted DefaultValue
    // New Identity, Deleted Identity

    // (not implemented) Trigger

    // SuspectedTableRename - if remove/add table but the internal schema is exactly the asame, suspect table rename
}
