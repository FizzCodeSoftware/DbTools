using FizzCode.LightWeight.AdoNet;

namespace FizzCode.DbTools;
public class SqLiteVersion : SqlEngineVersion
{
    internal SqLiteVersion(string uniqueName, string versionString, string versionNumber)
        : base(SqlEngine.SqLite, uniqueName, versionString, versionNumber, "System.Data.SQLite")
    {
    }

    public static SqLiteVersion SqLite3 { get; } = new SqLiteVersion(nameof(SqLite3), "3", "3");
}