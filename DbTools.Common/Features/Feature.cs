namespace FizzCode.DbTools.Common
{
    using System.Collections.Generic;

    public class Feature
    {
        public Feature(string name)
        {
            Name = name;
            Support = new Dictionary<SqlVersion, FeatureSupport>();
        }

        public string Name { get; set; }
        public Dictionary<SqlVersion, FeatureSupport> Support { get; set; }

        public void Add(SqlVersion version, Support support, string description = null)
        {
            Support.Add(version, new FeatureSupport(support, description));
        }

        public void Add(List<SqlVersion> versions, Support support, string description = null)
        {
            foreach(var version in versions)
                Support.Add(version, new FeatureSupport(support, description));
        }
    }
}
