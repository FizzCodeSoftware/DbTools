namespace FizzCode.DbTools.DataDefinition.Oracle12c
{
    using System.Globalization;

    public abstract class OracleType : SqlTypeInfo
    {
        public virtual bool IsLengthMandatory => HasLength;

        public virtual bool IsScaleMandatory => HasScale;

        public override string SqlDataType => base.SqlDataType.ToUpper(CultureInfo.InvariantCulture);
    }
}