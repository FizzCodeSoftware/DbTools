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

        // TODO move to converter?
        public SqlType PreferredType
        {
            get
            {
                if (Keys.Count == 1)
                    return Values.First();

                if (Keys.Any(k => SqlEngines.GetVersions<IGenericDialect>().Contains(k)))
                    return this[SqlEngines.GetLatestVersion<IGenericDialect>()];

                return null;
            }
        }

        public string Describe()
        {
            if (Keys.Any(k => SqlEngines.GetVersions<IGenericDialect>().Contains(k)))
            {
                return Describe(SqlEngines.GetLatestVersion<IGenericDialect>());
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