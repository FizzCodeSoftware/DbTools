using System.Collections;
using System.Collections.Generic;

namespace FizzCode.DbTools.DataDefinition.Base;
public class SqlEngineVersionSpecificProperty : IEnumerable<SqlEngineVersionSpecificProperty>
{
    public SqlEngineVersionSpecificProperty(SqlEngineVersion version, string name, string value)
    {
        Version = version;
        Name = name;
        Value = value;
    }

    public SqlEngineVersion Version { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }

    public string Key => GetKey(this);

    public static string GetKey(SqlEngineVersionSpecificProperty property)
    {
        return GetKey(property.Version, property.Name);
    }

    public static string GetKey(SqlEngineVersion version, string name)
    {
        return version.ToString() + "/" + name;
    }

    public IEnumerator<SqlEngineVersionSpecificProperty> GetEnumerator()
    {
        yield return this;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        yield return this;
    }
}
