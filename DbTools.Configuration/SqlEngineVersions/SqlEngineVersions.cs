using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FizzCode.DbTools;
public static class SqlEngineVersions
{
    static SqlEngineVersions()
    {
        var versionClassTypes = typeof(SqlEngineVersions).Assembly.GetTypes().Where(x => !x.IsAbstract && x.IsClass && typeof(SqlEngineVersion).IsAssignableFrom(x));
        foreach (var cls in versionClassTypes)
        {
            var staticInstanceProperties = cls.GetProperties(BindingFlags.Static | BindingFlags.Public).Where(x => x.PropertyType == cls);
            foreach (var prop in staticInstanceProperties)
            {
                var version = prop.GetValue(null) as SqlEngineVersion;
                AllVersions.Add(version);
            }
        }
    }

    public static SqlEngineVersion GetSqlEngineVersion(this LightWeight.AdoNet.NamedConnectionString connectionString)
    {
        var versionsByType = AllVersions.GroupBy(x => x.GetType());
        foreach (var group in versionsByType)
        {
            var matches = group
                .Where(version => string.Equals(connectionString.ProviderName, version.ProviderName, StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            if (matches.Count > 0)
            {
                return matches.Find(x => string.Equals(x.VersionString, connectionString.Version, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        return null;
    }

    public static List<SqlEngineVersion> AllVersions { get; } = [];

    public static SqlEngineVersion GetLatestVersionOfDialect<T>() where T : SqlEngineVersion
    {
        var lastVersion = AllVersions.Last(v => v is T);
        return lastVersion;
    }

    public static List<SqlEngineVersion> GetLatestExecutableVersions()
    {
        var latestVersions = AllVersions
            .Where(v => v is not GenericVersion)
            .GroupBy(t => t.GetType())
            .SelectMany(t => new[] { t.Last() })
            .ToList();

        return latestVersions.ToList();
    }

    public static List<SqlEngineVersion> GetAllVersions<T>()
        where T : SqlEngineVersion
    {
        var result = AllVersions.Where(v => v is T).ToList();
        return result;
    }

    public static SqlEngineVersion GetVersion<T>(string versionString)
        where T : SqlEngineVersion
    {
        var result = AllVersions.Find(v => typeof(T).IsAssignableFrom(v.GetType()) && v.VersionString == versionString);
        return result;
    }

    public static SqlEngineVersion GetVersion(string sqlType)
    {
        var result = AllVersions.Find(v => v.UniqueName == sqlType);
        return result;
    }
}