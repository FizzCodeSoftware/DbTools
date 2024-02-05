using FizzCode.LightWeight.AdoNet;

namespace FizzCode.DbTools;
public class GenericVersion : SqlEngineVersion
{
    internal GenericVersion(string uniqueName, string versionString, string versionNumber)
        : base(AdoNetEngine.GenericSql, uniqueName, versionString, versionNumber, null)
    {
    }

    public static GenericVersion Generic1 { get; } = new GenericVersion(nameof(Generic1), "1", "1");
}