using FizzCode.LightWeight.AdoNet;

namespace FizzCode.DbTools;
public class MySqlVersion : SqlEngineVersion
{
    internal MySqlVersion(string uniqueName, string versionString, string versionNumber)
        : base(AdoNetEngine.MySql, uniqueName, versionString, versionNumber, "MySql.Data.MySqlClient")
    {
    }

    public static MySqlVersion Generic { get; } = new MySqlVersion(nameof(Generic), null, null);
}