namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections;
    using System.Collections.Generic;

    public class SqlEngineVersionSpecificProperties : IEnumerable<SqlEngineVersionSpecificProperty>
    {
        private readonly Dictionary<string, SqlEngineVersionSpecificProperty> _properties = new();

        public string this[SqlEngineVersion version, string name]
        {
            get => _properties[SqlEngineVersionSpecificProperty.GetKey(version, name)].Value;
            set
            {
                var key = SqlEngineVersionSpecificProperty.GetKey(version, name);
                if (!_properties.ContainsKey(key))
                    Add(new SqlEngineVersionSpecificProperty(version, name, value));
                else
                    _properties[key].Value = value;
            }
        }

        public void Add(SqlEngineVersionSpecificProperty property)
        {
            _properties.Add(property.Key, property);
        }

        public void AddRange(IEnumerable<SqlEngineVersionSpecificProperty> properties)
        {
            foreach (var property in properties)
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

        public bool Contains(SqlEngineVersion version, string name)
        {
            return _properties.ContainsKey(SqlEngineVersionSpecificProperty.GetKey(version, name));
        }
    }
}
