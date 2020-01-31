namespace FizzCode.DbTools.DataDefinition.MsSql2016
{
    public abstract class MsSqlType2016 : MsSqlType
    {
    }

    public abstract class MsSqlType : SqlTypeInfo
    {
        public virtual bool IsMaxLengthAllowed { get; }
    }


    public class Char : MsSqlType2016
    {
        public override bool HasLength => true;
        public override bool HasScale => false;
        public override bool IsMaxLengthAllowed => false;
    }

    public class NChar : MsSqlType2016
    {
        public override bool HasLength => true;
        public override bool HasScale => false;
        public override bool IsMaxLengthAllowed => false;
    }

    public class VarChar : MsSqlType2016
    {
        public override bool HasLength => true;
        public override bool HasScale => false;
        public override bool IsMaxLengthAllowed => true;
    }

    public class NVarChar : MsSqlType2016
    {
        public override bool HasLength => true;
        public override bool HasScale => false;
        public override bool IsMaxLengthAllowed => true;
    }

    public class Text : MsSqlType2016
    {
        public override bool HasLength => true;
        public override bool HasScale => false;
        public override bool IsMaxLengthAllowed => true;
        public override bool Deprecated => true;
    }

    public class NText : MsSqlType2016
    {
        public override bool HasLength => true;
        public override bool HasScale => false;
        public override bool IsMaxLengthAllowed => true;
        public override bool Deprecated => true;
    }

    public class Bit : MsSqlType2016
    {
        public override bool HasLength => false;
        public override bool HasScale => false;
        public override bool IsMaxLengthAllowed => false;
    }


    public class TinyInt : MsSqlType2016
    {
        public override bool HasLength => false;
        public override bool HasScale => false;
        public override bool IsMaxLengthAllowed => false;
    }

    public class SmallInt : MsSqlType2016
    {
        public override bool HasLength => false;
        public override bool HasScale => false;
        public override bool IsMaxLengthAllowed => false;
    }

    public class Int : MsSqlType2016
    {
        public override bool HasLength => false;
        public override bool HasScale => false;
        public override bool IsMaxLengthAllowed => false;
    }

    public class BigInt : MsSqlType2016
    {
        public override bool HasLength => false;
        public override bool HasScale => false;
        public override bool IsMaxLengthAllowed => false;
    }

    public class Decimal : MsSqlType2016
    {
        public override bool HasLength => true;
        public override bool HasScale => true;
        public override bool IsMaxLengthAllowed => false;
    }

    public class Numeric : MsSqlType2016
    {
        public override bool HasLength => true;
        public override bool HasScale => true;
        public override bool IsMaxLengthAllowed => false;
    }

    public class Money : MsSqlType2016
    {
        public override bool HasLength => false;
        public override bool HasScale => false;
        public override bool IsMaxLengthAllowed => false;
    }

    public class SmallMoney : MsSqlType2016
    {
        public override bool HasLength => false;
        public override bool HasScale => false;
        public override bool IsMaxLengthAllowed => false;
    }

    public class Float : MsSqlType2016
    {
        public override bool HasLength => false;
        public override bool HasScale => false;
        public override bool IsMaxLengthAllowed => false;
    }

    public class Real : MsSqlType2016
    {
        public override bool HasLength => false;
        public override bool HasScale => false;
        public override bool IsMaxLengthAllowed => false;
    }


    public class Date : MsSqlType2016
    {
        public override bool HasLength => false;
        public override bool HasScale => false;
        public override bool IsMaxLengthAllowed => false;
    }

    public class Time : MsSqlType2016
    {
        public override bool HasLength => true;
        public override bool HasScale => false;
        public override bool IsMaxLengthAllowed => false;
    }

    public class DateTime : MsSqlType2016
    {
        public override bool HasLength => false;
        public override bool HasScale => false;
        public override bool IsMaxLengthAllowed => false;
    }

    public class DateTime2 : MsSqlType2016
    {
        public override bool HasLength => true;
        public override bool HasScale => false;
        public override bool IsMaxLengthAllowed => false;
    }

    public class DateTimeOffset : MsSqlType2016
    {
        public override bool HasLength => true;
        public override bool HasScale => false;
        public override bool IsMaxLengthAllowed => false;
    }
    public class SmallDateTime : MsSqlType2016
    {
        public override bool HasLength => false;
        public override bool HasScale => false;
        public override bool IsMaxLengthAllowed => false;
    }

    public class Binary : MsSqlType2016
    {
        public override bool HasLength => true;
        public override bool HasScale => false;
        public override bool IsMaxLengthAllowed => false;
    }

    public class VarBinary : MsSqlType2016
    {
        public override bool HasLength => true;
        public override bool HasScale => false;
        public override bool IsMaxLengthAllowed => true;
    }

    public class Image : MsSqlType2016
    {
        public override bool HasLength => false;
        public override bool HasScale => false;
        public override bool IsMaxLengthAllowed => false;
        public override bool Deprecated => true;
    }


    public class IniqueIdentifier : MsSqlType2016
    {
        public override bool HasLength => false;
        public override bool HasScale => false;
        public override bool IsMaxLengthAllowed => false;
    }

    public class Xml : MsSqlType2016
    {
        public override bool HasLength => false;
        public override bool HasScale => false;
        public override bool IsMaxLengthAllowed => false;
    }

    public class UniqueIdentifier : MsSqlType2016
    {
        public override bool HasLength => false;
        public override bool HasScale => false;
        public override bool IsMaxLengthAllowed => false;
    }
}