namespace FizzCode.DbTools.Common
{
    using System.Collections.Generic;

    public class Feature
    {
        public Feature(string name)
        {
            Name = name;
            Support = new Dictionary<SqlEngineVersion, FeatureSupport>();
        }

        public string Name { get; set; }
        public Dictionary<SqlEngineVersion, FeatureSupport> Support { get; set; }

        public void Add(SqlEngineVersion version, Support support, string description = null)
        {
            Support.Add(version, new FeatureSupport(support, description));
        }

        public void Add(List<SqlEngineVersion> versions, Support support, string description = null)
        {
            foreach (var version in versions)
                Support.Add(version, new FeatureSupport(support, description));
        }
    }
}
