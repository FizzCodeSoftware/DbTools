namespace FizzCode.DbTools.Common
{
    public sealed class Features
    {
        static Features()
        {
            _features.Add("ReadDdl");
            _features["ReadDdl"].Add(SqlDialect.SqLite, Support.NotSupported, "No known way to read DDL with SqLite in memory.");
            _features["ReadDdl"].Add(SqlDialect.Oracle, Support.NotImplementedYet);
        }

        private Features()
        {
        }

        public static Features Instance { get; } = new Features();

        private static readonly FeatureList _features = new FeatureList();

        public FeatureSupport this[SqlDialect sqlDialect, string name]
        {
            get
            {
                if (_features.ContainsKey(name) && _features[name].Support.ContainsKey(sqlDialect))
                    return _features[name].Support[sqlDialect];

                return new FeatureSupport(Support.Unknown, null);
            }
        }
    }
}
