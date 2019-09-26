namespace FizzCode.DbTools.DataDefinition
{
    /// <summary>
    /// Enumeration of possible sql data types.
    /// </summary>
    /// <remarks>Keeping close to <see cref="System.Data.SqlTypes"/>.</remarks>
#pragma warning disable CA1720 // Identifier contains type name
    public enum SqlType
    {
        Unknown,

        Boolean,

        Char,
        NChar,
        Varchar,
        NVarchar,

        DateTime,
        DateTimeOffset,
        Date,

        Byte,
        Int16,
        Int32,
        Int64,

        Decimal,
        Single,
        Double,
        Money,

        Xml,
        Guid,
        Binary,

        Image,
        VarBinary,
        NText
    }
#pragma warning restore CA1720 // Identifier contains type name
}