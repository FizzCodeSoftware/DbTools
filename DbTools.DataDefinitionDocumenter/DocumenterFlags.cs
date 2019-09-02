using System;

namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    [Flags]
    public enum DocumenterFlags
    {
        None,
        NoDetailedForeignKeys,
        NoDetailedIndexes,
        NoInternalDataTypes,
    }
}
