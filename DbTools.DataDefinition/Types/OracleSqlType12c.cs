namespace FizzCode.DbTools.DataDefinition.Oracle12c
{
    using System;
    using System.Globalization;

    public abstract class OracleSqlType : SqlTypeInfo
    {
        public bool IsLengthMandatory
        {
            get
            {
                if (IsLengthMandatoryInternal.HasValue)
                    return IsLengthMandatoryInternal.Value;
                else
                    if (HasLength)
                        return false;
                else
                    throw new Exception();
            }
        }

        public bool IsScaleMandatory
        {
            get
            {
                if (IsScaleMandatoryInternal.HasValue)
                    return IsScaleMandatoryInternal.Value;
                else
                    if (HasScale)
                        return true;
                else
                    throw new Exception();
            }
        }

        protected virtual bool? IsLengthMandatoryInternal { get; }

        protected virtual bool? IsScaleMandatoryInternal { get; }

        public override string SqlDataType => base.SqlDataType.ToUpper(CultureInfo.InvariantCulture);
    }

    public abstract class OracleSqlType12c : OracleSqlType
    {
    }


    public class Char : OracleSqlType12c
    {
        public override bool HasLength => true;

        public override bool HasScale => false;
    }

    public class NChar : OracleSqlType12c
    {
        public override bool HasLength => true;

        public override bool HasScale => false;
    }

    public class VarChar : OracleSqlType12c
    {
        public override bool HasLength => true;

        public override bool HasScale => false;
    }

    public class VarChar2 : OracleSqlType12c
    {
        public override bool HasLength => true;

        public override bool HasScale => false;
    }

    public class NVarChar2 : OracleSqlType12c
    {
        public override bool HasLength => true;

        public override bool HasScale => false;
    }

    public class Blob : OracleSqlType12c
    {
        public override bool HasLength => false;

        public override bool HasScale => false;
    }

    public class Clob : OracleSqlType12c
    {
        public override bool HasLength => false;

        public override bool HasScale => false;
    }

    public class NClob : OracleSqlType12c
    {
        public override bool HasLength => false;

        public override bool HasScale => false;
    }

    public class Bfile : OracleSqlType12c
    {
        public override bool HasLength => false;

        public override bool HasScale => false;
    }

    public class Long : OracleSqlType12c
    {
        public override bool HasLength => false;

        public override bool HasScale => false;
        public override bool Deprecated => true;
    }

    public class LongRaw : OracleSqlType12c
    {
        public override bool HasLength => false;

        public override bool HasScale => false;
        public override bool Deprecated => true;
    }

    public class Number  : OracleSqlType12c
    {
        public override bool HasLength => true;

        public override bool HasScale => true;

        protected override bool? IsLengthMandatoryInternal => false;
        protected override bool? IsScaleMandatoryInternal => false;
    }

    public class BinaryFloat : OracleSqlType12c
    {
        public override bool HasLength => false;

        public override bool HasScale => false;
    }

    public class BinaryDouble : OracleSqlType12c
    {
        public override bool HasLength => false;

        public override bool HasScale => false;
    }

    public class Date : OracleSqlType12c
    {
        public override bool HasLength => false;

        public override bool HasScale => false;
    }

    public class TimeStampWithTimeZone : OracleSqlType12c
    {
        public override bool HasLength => false;

        public override bool HasScale => false;

        public override string SqlDataType => "TIMESTAMP WITH TIME ZONE";
    }

    public class TimeStampWithLocalTimeZone : OracleSqlType12c
    {
        public override bool HasLength => false;

        public override bool HasScale => false;

        public override string SqlDataType => "TIMESTAMP WITH LOCAL TIME ZONE";
    }

    public class XmlType : OracleSqlType12c
    {
        public override bool HasLength => false;

        public override bool HasScale => false;
    }

    public class UriType : OracleSqlType12c
    {
        public override bool HasLength => false;

        public override bool HasScale => false;
    }

}