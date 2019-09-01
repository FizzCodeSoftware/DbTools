namespace FizzCode.DbTools.Common
{
    using System.Collections.Generic;

    public class FeatureList : Dictionary<string, Feature>
    {
        public Feature Add(string name)
        {
            var feature = new Feature(name);
            Add(name, feature);
            return feature;
        }
    }
}
