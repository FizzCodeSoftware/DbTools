namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.Configuration;

    public class SqlTypes : Dictionary<SqlVersion, SqlType>
    {
        public void SetAllNullable(bool isNullable)
        {
            foreach (var sqlType in Values)
            {
                sqlType.IsNullable = isNullable;
            }
        }

        private SqlVersion GetVersion()
        {
            if (Keys.Any(k => SqlVersions.GetVersions<IGenericDialect>().Contains(k)))
            {
                return SqlVersions.GetLatestVersion<IGenericDialect>();
            }

            return Keys.Last();
        }

        public string Describe(SqlVersion preferredVersion = null)
        {
            var version = preferredVersion;
            if (preferredVersion == null)
                version = GetVersion();

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