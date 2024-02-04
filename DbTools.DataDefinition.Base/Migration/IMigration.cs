namespace FizzCode.DbTools.DataDefinition.Base.Migration;

public interface IMigration
{ }

// Other / all cases
// New PK, Deleted PK, Renamed PK - changed PK?

// New TableDescription, Deleted TableDescription
// New ColumnDescription, Deleted ColumnDescription

// New DefaultValue, Deleted DefaultValue
// New Identity, Deleted Identity

// (not implemented) Trigger

// SuspectedTableRename - if remove/add table but the internal schema is exactly the asame, suspect table rename
