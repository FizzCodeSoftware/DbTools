namespace FizzCode.DbTools.Configuration
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

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

        public static List<SqlEngineVersion> AllVersions { get; } = new List<SqlEngineVersion>();

        public static SqlEngineVersion GetLatestVersionOfDialect<T>() where T : SqlEngineVersion
        {
            var lastVersion = AllVersions.Last(v => v is T);
            return lastVersion;
        }

        public static List<SqlEngineVersion> GetLatestExecutableVersions()
        {
            var latestVersions = AllVersions
                .Where(v => !(v is GenericVersion))
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
}