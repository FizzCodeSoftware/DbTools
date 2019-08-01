namespace FizzCode.DbTools.DataDefinition
{
    /// <summary>
    /// Enumeration of possible sql data types.
    /// </summary>
    /// <remarks>Keeping close to <see cref="System.Data.SqlTypes"/>.</remarks>
    public enum SqlType
    {
        Unknown,

        Boolean,

        Char,
        NChar,
        Varchar,
        NVarchar,

        DateTime,
        Date,

        Byte,
        Int16,
        Int32,
        Int64,

        Decimal,
        Double,

        Xml,
        Guid,
        Binary,

        Image,
        VarBinary,
        NText
    }
}