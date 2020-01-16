namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.Common;
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
                sqlTypes.Add(kvp.Key, kvp.Value.CopyTo(new SqlType()));
            }

            return sqlTypes;
        }
    }

    public class SqlColumn
    {
        public SqlTable Table { get; set; }
        public string Name { get; set; }
        public SqlTypes Types { get; set; }

        private List<SqlColumnProperty> _properties;
        public List<SqlColumnProperty> Properties => _properties ?? (_properties = new List<SqlColumnProperty>());

        public override string ToString()
        {
            return $"{Name} {Types.Describe()} on {Table.SchemaAndTableName}";
        }

        public SqlColumn CopyTo(SqlColumn column)
        {
            column.Name = Name;
            column.Types = Types.CopyTo(new SqlTypes());
            column.Table = Table;
            return column;
        }

        public SqlColumn SetPK()
        {
            Table.SetPK(this);
            return this;
        }

        public SqlColumn SetIdentity()
        {
            Properties.Add(new Identity(this));
            return this;
        }

        public bool HasProperty<T>()
            where T : SqlColumnProperty
        {
            return Properties.Any(x => x is T);
        }
    }
}