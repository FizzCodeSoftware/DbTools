namespace FizzCode.DbTools.DataDefinition.Oracle12c
{
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

    public class SqlBFile : OracleType12c
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

    public class SqlNumber : OracleType12c
    {
        public override bool HasLength => true;

        public override bool HasScale => true;

        public override bool IsLengthMandatory => false;
        public override bool IsScaleMandatory => false;
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