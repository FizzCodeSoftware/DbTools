using FizzCode.DbTools.DataDefinition.Base.Interfaces;
using FizzCode.DbTools.Factory.Collections;

namespace FizzCode.DbTools.DataDefinition.Base;
public class TypeMappers(ITypeMapperFactory factory)
    : SqlEngineVersionFactoryDictionary<ITypeMapper, ITypeMapperFactory>(factory)
{
    protected override ITypeMapper Create(SqlEngineVersion version)
    {
        return _factory.GetTypeMapper(version);
    }

    public void Add(ITypeMapper typeMapper)
    {
        Add(typeMapper.SqlVersion, typeMapper);
    }
}