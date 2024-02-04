using System.Globalization;
using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDefinition.Oracle12c;
public abstract class OracleType : SqlTypeInfo
{
    public virtual bool IsLengthMandatory => HasLength;

    public virtual bool IsScaleMandatory => HasScale;

    public override string SqlDataType => base.SqlDataType.ToUpper(CultureInfo.InvariantCulture);
}