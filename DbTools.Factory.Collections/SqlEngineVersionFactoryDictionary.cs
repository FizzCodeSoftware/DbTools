namespace FizzCode.DbTools.Factory.Collections;

public abstract class SqlEngineVersionFactoryDictionary<T, TIFactory> : Dictionary<SqlEngineVersion, T>
{
    protected readonly TIFactory _factory;

    public SqlEngineVersionFactoryDictionary(TIFactory factory)
    {
        _factory = factory;
    }

    public new T this[SqlEngineVersion version]
    {
        get
        {
            if (!ContainsKey(version))
                Add(version, Create(version));

            return base[version];
        }
    }

    protected abstract T Create(SqlEngineVersion version);
}