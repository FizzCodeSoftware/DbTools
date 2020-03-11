namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections;
    using System.Collections.Generic;
    using FizzCode.DbTools.Configuration;

    public class SqlEngineVersionSpecificProperties : IEnumerable<SqlEngineVersionSpecificProperty>
    {
        private readonly Dictionary<string, SqlEngineVersionSpecificProperty> _properties = new Dictionary<string, SqlEngineVersionSpecificProperty>();

        public SqlEngineVersionSpecificProperty this[SqlEngineVersion version, string name]
        {
            get
            {
                return _properties[version.ToString() + "/" + name];
            }
        }

        public void Add(SqlEngineVersionSpecificProperty property)
        {
            _properties.Add(property.Key, property);
        }

        public void AddRange(IEnumerable<SqlEngineVersionSpecificProperty> properties)
        {
            foreach(var property in properties)
                Add(property);
        }

        public IEnumerator<SqlEngineVersionSpecificProperty> GetEnumerator()
        {
            foreach (var value in _properties.Values)
                yield return value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var value in _properties.Values)
                yield return value;
        }

        internal void Add(IEnumerable<SqlEngineVersionSpecificProperty> properties)
        {
            foreach (var property in properties)
            {
                Add(property);
            }
        }
    }
}
