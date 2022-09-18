namespace FizzCode.DbTools.DataDefinition.Base
{
    using System.Collections.Generic;
    using System.Linq;

    public class SqlTypes : Dictionary<SqlEngineVersion, SqlType>
    {
        public void SetAllNullable(bool isNullable)
        {
            foreach (var sqlType in Values)
            {
                sqlType.IsNullable = isNullable;
            }
        }

        private SqlEngineVersion GetVersion()
        {
            if (Keys.Any(k => SqlEngineVersions.GetAllVersions<GenericVersion>().Contains(k)))
            {
                return SqlEngineVersions.GetLatestVersionOfDialect<GenericVersion>();
            }

            return Keys.Last();
        }

        public string Describe(SqlEngineVersion? preferredVersion = null)
        {
            var version = preferredVersion;
            version ??= GetVersion();

            return this[version].ToString();
        }

        public SqlTypes CopyTo(SqlTypes sqlTypes)
        {
            foreach (var kvp in this)
            {
                sqlTypes.Add(kvp.Key, kvp.Value.Copy());
            }

            return sqlTypes;
        }
    }
}