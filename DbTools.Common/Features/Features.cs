namespace FizzCode.DbTools.Common
{
    using FizzCode.DbTools.Configuration;

    public static class Features
    {
        static Features()
        {
            _features.Add("ReadDdl");
            _features["ReadDdl"].Add(SqlVersions.SqLite3, Support.NotSupported, "No known way to read DDL with SqLite in memory.");
        }

        private static readonly FeatureList _features = new FeatureList();

        public static FeatureSupport GetSupport(SqlVersion version, string name)
        {
            if (_features.ContainsKey(name) && _features[name].Support.ContainsKey(version))
                return _features[name].Support[version];

            return new FeatureSupport(Support.Unknown, null);
        }
    }
}