namespace FizzCode.DbTools.DataDefinition.Oracle12c
{
    using System;
    using System.Globalization;

    public abstract class OracleType : SqlTypeInfo
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


    public class SqlChar : OracleType12c
    {
        public override bool HasLength => true;

        public override bool HasScale => false;
    }

    public class SqlNChar : OracleType12c
    {
        public override bool HasLength => true;

        public override bool HasScale => false;
    }

    public class SqlVarChar : OracleType12c
    {
        public override bool HasLength => true;

        public override bool HasScale => false;
    }

    public class SqlVarChar2 : OracleType12c
    {
        public override bool HasLength => true;

        public override bool HasScale => false;
    }

    public class SqlNVarChar2 : OracleType12c
    {
        public override bool HasLength => true;

        public override bool HasScale => false;
    }

    public class SqlBlob : OracleType12c
    {
        public override bool HasLength => false;

        public override bool HasScale => false;
    }

    public class SqlClob : OracleType12c
    {
        public override bool HasLength => false;

        public override bool HasScale => false;
    }

    public class SqlNClob : OracleType12c
    {
        public override bool HasLength => false;

        public override bool HasScale => false;
    }

    public class SqlBfile : OracleType12c
    {
        public override bool HasLength => false;

        public override bool HasScale => false;
    }

    public class SqlLong : OracleType12c
    {
        public override bool HasLength => false;

        public override bool HasScale => false;
        public override bool Deprecated => true;
    }

    public class SqlLongRaw : OracleType12c
    {
        public override bool HasLength => false;

        public override bool HasScale => false;
        public override bool Deprecated => true;
    }

    public class SqlNumber  : OracleType12c
    {
        public override bool HasLength => true;

        public override bool HasScale => true;

        protected override bool? IsLengthMandatoryInternal => false;
        protected override bool? IsScaleMandatoryInternal => false;
    }

    public class SqlBinaryFloat : OracleType12c
    {
        public override bool HasLength => false;

        public override bool HasScale => false;
    }

    public class SqlBinaryDouble : OracleType12c
    {
        public override bool HasLength => false;

        public override bool HasScale => false;
    }

    public class SqlDate : OracleType12c
    {
        public override bool HasLength => false;

        public override bool HasScale => false;
    }

    public class SqlTimeStampWithTimeZone : OracleType12c
    {
        public override bool HasLength => false;

        public override bool HasScale => false;

        public override string SqlDataType => "TIMESTAMP WITH TIME ZONE";
    }

    public class SqlTimeStampWithLocalTimeZone : OracleType12c
    {
        public override bool HasLength => false;

        public override bool HasScale => false;

        public override string SqlDataType => "TIMESTAMP WITH LOCAL TIME ZONE";
    }

    public class SqlXmlType : OracleType12c
    {
        public override bool HasLength => false;

        public override bool HasScale => false;
    }

    public class SqlUriType : OracleType12c
    {
        public override bool HasLength => false;

        public override bool HasScale => false;
    }

}