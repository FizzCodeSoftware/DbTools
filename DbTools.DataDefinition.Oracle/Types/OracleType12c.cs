#pragma warning disable CA1720 // Identifier contains type name

namespace FizzCode.DbTools.DataDefinition
{
    using FizzCode.DbTools.DataDefinition.Oracle12c;

    public abstract class OracleType12c : OracleType
    {
        public static SqlChar Char { get; } = new SqlChar();
        public static SqlNChar NChar { get; } = new SqlNChar();
        public static SqlVarChar VarChar { get; } = new SqlVarChar();
        public static SqlVarChar2 VarChar2 { get; } = new SqlVarChar2();
        public static SqlNVarChar2 NVarChar2 { get; } = new SqlNVarChar2();
        public static SqlBlob Blob { get; } = new SqlBlob();
        public static SqlClob Clob { get; } = new SqlClob();
        public static SqlNClob NClob { get; } = new SqlNClob();
        public static SqlBfile BFile { get; } = new SqlBfile();
        public static SqlLong Long { get; } = new SqlLong();
        public static SqlLongRaw LongRaw { get; } = new SqlLongRaw();
        public static SqlNumber Number { get; } = new SqlNumber();
        public static SqlBinaryFloat BinaryFloat { get; } = new SqlBinaryFloat();
        public static SqlBinaryDouble BinaryDouble { get; } = new SqlBinaryDouble();
        public static SqlDate Date { get; } = new SqlDate();
        public static SqlTimeStampWithTimeZone TimeStampWithTimeZone { get; } = new SqlTimeStampWithTimeZone();
        public static SqlTimeStampWithLocalTimeZone TimeStampWithLocalTimeZone { get; } = new SqlTimeStampWithLocalTimeZone();

        public static SqlXmlType XmlType { get; } = new SqlXmlType();
        public static SqlUriType UriType { get; } = new SqlUriType();
    }
}
