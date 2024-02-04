using System.Collections.Generic;

namespace FizzCode.DbTools.Common;
public class FeatureList : Dictionary<string, Feature>
{
    public Feature Add(string name)
    {
        var feature = new Feature(name);
        Add(name, feature);
        return feature;
    }
}
