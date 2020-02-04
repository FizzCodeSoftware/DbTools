#pragma warning disable CA1720 // Identifier contains type name

namespace FizzCode.DbTools.DataDefinition
{
    using FizzCode.DbTools.DataDefinition.MsSql2016;

    public abstract class MsSqlType2016 : MsSqlType
    {
        public static SqlChar Char { get; } = new SqlChar();
        public static SqlNChar NChar { get; } = new SqlNChar();
        public static SqlVarChar VarChar { get; } = new SqlVarChar();
        public static SqlNVarChar NVarChar { get; } = new SqlNVarChar();
        public static SqlTinyInt TinyInt { get; } = new SqlTinyInt();
        public static SqlSmallInt SmallInt { get; } = new SqlSmallInt();
        public static SqlBigInt BigInt { get; } = new SqlBigInt();
        public static SqlInt Int { get; } = new SqlInt();
        public static SqlBit Bit { get; } = new SqlBit();
        public static SqlDecimal Decimal { get; } = new SqlDecimal();
        public static SqlNumeric Numeric { get; } = new SqlNumeric();
        public static SqlMoney Money { get; } = new SqlMoney();
        public static SqlSmallMoney SmallMoney { get; } = new SqlSmallMoney();
        public static SqlDate Date { get; } = new SqlDate();
        public static SqlTime Time { get; } = new SqlTime();
        public static SqlDateTime DateTime { get; } = new SqlDateTime();
        public static SqlSmallDateTime SmallDateTime { get; } = new SqlSmallDateTime();
        public static SqlDateTime2 DateTime2 { get; } = new SqlDateTime2();
        public static SqlDateTimeOffset DateTimeOffset { get; } = new SqlDateTimeOffset();
        public static SqlFloat Float { get; } = new SqlFloat();
        public static SqlReal Real { get; } = new SqlReal();
        public static SqlBinary Binary { get; } = new SqlBinary();
        public static SqlVarBinary VarBinary { get; } = new SqlVarBinary();
        public static SqlImage Image { get; } = new SqlImage();
        public static SqlText Text { get; } = new SqlText();
        public static SqlNText NText { get; } = new SqlNText();
        public static SqlUniqueIdentifier UniqueIdentifier { get; } = new SqlUniqueIdentifier();
        public static SqlXml Xml { get; } = new SqlXml();
    }

    public static class MsSqlType2016Extensions
    {
        public static bool IsInt(this SqlType sqlType)
        {
            return sqlType.SqlTypeInfo is SqlInt;
        }

        public static bool IsBit(this SqlType sqlType)
        {
            return sqlType.SqlTypeInfo is SqlBit;
        }
    }
}