namespace FizzCode.DbTools.Common
{
    public static class Features
    {
        static Features()
        {
            _features.Add("ReadDdl");
            _features["ReadDdl"].Add(SqLiteVersion.SqLite3, Support.NotSupported, "No known way to read DDL with SqLite in memory.");
            _features.Add("Schema");
            _features["Schema"].Add(SqLiteVersion.SqLite3, Support.NotSupported, "SqLite does not support Schemas.");
            _features.Add("ColumnLength");
            _features["ColumnLength"].Add(SqLiteVersion.SqLite3, Support.NotSupported, "SqLite does not support any datatype Length.");
            _features.Add("ColumnScale");
            _features["ColumnScale"].Add(SqLiteVersion.SqLite3, Support.NotSupported, "SqLite does not support any datatype Length.");
        }

        private static readonly FeatureList _features = new();

        public static FeatureSupport GetSupport(SqlEngineVersion version, string name)
        {
            if (_features.ContainsKey(name) && _features[name].Support.ContainsKey(version))
                return _features[name].Support[version];

            return new FeatureSupport(Support.Unknown, null);
        }
    }
}