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

        public string Describe()
        {
            if (Keys.Any(k => SqlVersions.GetVersions<IGenericDialect>().Contains(k)))
            {
                return Describe(SqlVersions.GetLatestVersion<IGenericDialect>());
            }

            return Describe(Keys.Last());
        }

        public string Describe(SqlVersion preferredVersion)
        {
            return this[preferredVersion].ToString();
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