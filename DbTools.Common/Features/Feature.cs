using System.Collections.Generic;

namespace FizzCode.DbTools.Common;
public class Feature
{
    public Feature(string name)
    {
        Name = name;
        Support = [];
    }

    public string Name { get; set; }
    public FeatureSupports Support { get; set; }

    public void Add(SqlEngineVersion version, Support support, string description)
    {
        Support.Add(version, new FeatureSupport(support, description));
    }

    public void Add(List<SqlEngineVersion> versions, Support support, string description)
    {
        foreach (var version in versions)
            Support.Add(version, new FeatureSupport(support, description));
    }
}
