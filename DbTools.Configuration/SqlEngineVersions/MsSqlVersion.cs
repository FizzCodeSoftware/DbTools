using FizzCode.LightWeight.AdoNet;

namespace FizzCode.DbTools;
public class MsSqlVersion : SqlEngineVersion
{
    internal MsSqlVersion(string uniqueName, string versionString, string versionNumber)
        : base(SqlEngine.MsSql, uniqueName, versionString, versionNumber, "Microsoft.Data.SqlClient")
    {
    }

    public static MsSqlVersion MsSql2016 { get; } = new MsSqlVersion(nameof(MsSql2016), "2016", "13.0");
}