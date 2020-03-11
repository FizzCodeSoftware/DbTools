namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;
    using FizzCode.DbTools.Configuration;

    public class SqlEngineVersionSpecificProperties
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
    }
}
