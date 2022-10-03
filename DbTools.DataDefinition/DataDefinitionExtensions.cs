namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;
    using FizzCode.DbTools.DataDefinition.Base;

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

}