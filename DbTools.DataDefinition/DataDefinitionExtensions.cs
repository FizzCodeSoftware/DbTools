using System.Collections.Generic;
using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDefinition;
public static class DataDefinitionExtensions
{
    public static List<SqlEngineVersion> GetVersions(this IDatabaseDefinition dd)
    {
        var versions = new List<SqlEngineVersion>();
        if (dd.MainVersion != null)
            versions.Add(dd.MainVersion);

        if (dd.SecondaryVersions != null)
            versions.AddRange(dd.SecondaryVersions);

        return versions;
    }
}