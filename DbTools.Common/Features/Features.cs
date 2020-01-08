namespace FizzCode.DbTools.Common
{
    public static class Features
    {
        static Features()
        {
            _features.Add("ReadDdl");
            _features["ReadDdl"].Add(SqlDialect.SqLite, Support.NotSupported, "No known way to read DDL with SqLite in memory.");
        }

        private static readonly FeatureList _features = new FeatureList();

        public static FeatureSupport GetSupport(SqlDialect sqlDialect, string name)
        {
            if (_features.ContainsKey(name) && _features[name].Support.ContainsKey(sqlDialect))
                return _features[name].Support[sqlDialect];

            return new FeatureSupport(Support.Unknown, null);
        }
    }
}