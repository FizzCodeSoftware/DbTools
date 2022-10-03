namespace FizzCode.DbTools.DataDefinition.Base
{
    using FizzCode.DbTools.DataDefinition.Base.Interfaces;
    using FizzCode.DbTools.Factory.Collections;

    public class TypeMappers : SqlEngineVersionFactoryDictionary<ITypeMapper, ITypeMapperFactory>
    {
        public TypeMappers(ITypeMapperFactory factory)
            : base(factory)
        {
        }

        protected override ITypeMapper Create(SqlEngineVersion version)
        {
            return _factory.GetTypeMapper(version);
        }

        public void Add(ITypeMapper typeMapper)
        {
            Add(typeMapper.SqlVersion, typeMapper);
        }
    }
}