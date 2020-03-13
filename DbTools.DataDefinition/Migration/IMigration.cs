namespace FizzCode.DbTools.DataDefinition.Migration
{
#pragma warning disable CA1040 // Avoid empty interfaces
    public interface IMigration
#pragma warning restore CA1040 // Avoid empty interfaces
    { }

    // Other / all cases
    // New PK, Deleted PK, Renamed PK - changed PK?
    // New FK, Deleted FK, Renamed FK
    // FK NoCheck change - SqlEngineVersionSpecificProperty change
    // New Index, Deleted Index, Renamed Index - changed index?

    // New TableDescription, Deleted TableDescription
    // New ColumnDescription, Deleted ColumnDescription

    // New DefaultValue, Deleted DefaultValue
    // New Identity, Deleted Identity

    // (not implemented) Trigger

    // SuspectedTableRename - if remove/add table but the internal schema is exactly the asame, suspect table rename
}
