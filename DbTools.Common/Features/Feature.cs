namespace FizzCode.DbTools.Common
{
    using System.Collections.Generic;

    public class Feature
    {
        public Feature(string name)
        {
            Name = name;
            Support = new Dictionary<SqlDialect, FeatureSupport>();
        }

        public string Name { get; set; }
        public Dictionary<SqlDialect, FeatureSupport> Support { get; set; }

        public void Add(SqlDialect sqlDialect, Support support, string description = null)
        {
            Support.Add(sqlDialect, new FeatureSupport(support, description));
        }
    }
}
