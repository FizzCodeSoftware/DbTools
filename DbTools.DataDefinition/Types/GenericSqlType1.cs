namespace FizzCode.DbTools.DataDefinition.Generic1
{
    using System;

    public abstract class GenericSqlType1 : SqlTypeInfo
    {
        public static SqlChar Char { get; } = new SqlChar();
        public static SqlNChar NChar { get; } = new SqlNChar();
        public static SqlVarChar VarChar { get; } = new SqlVarChar();
        public static SqlNVarChar NVarChar { get; } = new SqlNVarChar();
        public static SqlFloatSmall FloatSmall { get; } = new SqlFloatSmall();
        public static SqlFloatLarge FloatLarge { get; } = new SqlFloatLarge();
        public static SqlBit Bit { get; } = new SqlBit();
        public static SqlByte Byte { get; } = new SqlByte();
        public static SqlInt16 Int16 { get; } = new SqlInt16();
        public static SqlInt32 Int32 { get; } = new SqlInt32();
        public static SqlInt64 Int64 { get; } = new SqlInt64();
        public static SqlNumber Number { get; } = new SqlNumber();
        public static SqlDate Date { get; } = new SqlDate();
        public static SqlTime Time { get; } = new SqlTime();
        public static SqlDateTime DateTime { get; } = new SqlDateTime();





    }
}

