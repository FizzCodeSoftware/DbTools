namespace FizzCode.DbTools.Common;

public static class Features
{
    public const string ReadDdl = "ReadDdl";
    public const string Schema = "Schema";
    public const string ColumnLength = "ColumnLength";
    public const string ColumnScale = "ColumnScale";
    public const string TableRecreation = "TableRecreation";

    static Features()
    {
        _features.Add(ReadDdl);
        _features[ReadDdl].Add(SqLiteVersion.SqLite3, Support.NotSupported, "No known way to read DDL with SqLite in memory.");
        _features.Add(Schema);
        _features[Schema].Add(SqLiteVersion.SqLite3, Support.NotSupported, "SqLite does not support Schemas.");
        _features.Add(ColumnLength);
        _features[ColumnLength].Add(SqLiteVersion.SqLite3, Support.NotSupported, "SqLite does not support any datatype Length.");
        _features.Add(ColumnScale);
        _features[ColumnScale].Add(SqLiteVersion.SqLite3, Support.NotSupported, "SqLite does not support any datatype Length.");
        _features.Add(TableRecreation);
        _features[TableRecreation].Add(MsSqlVersion.MsSql2016, Support.NotImplementedYet, "DbTools does not support table recreation (yet).");
        _features[TableRecreation].Add(OracleVersion.Oracle12c, Support.NotImplementedYet, "DbTools does not support table recreation (yet).");
    }

    private static readonly FeatureList _features = [];

    public static FeatureSupport GetSupport(SqlEngineVersion version, string name)
    {
        if (_features.TryGetValue(name, out var feature)
            && feature.Support.TryGetValue(version, out var value))
        {
            return value;
        }

        return new FeatureSupport(Support.Unknown, $"The support for the feature {name} for {version} is unknown.");
    }
}