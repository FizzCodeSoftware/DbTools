namespace FizzCode.DbTools.DataDefinition.Factory.Interfaces
{
    using FizzCode.DbTools.DataDefinition.Base.Interfaces;

    public interface ITypeMapperFactory
    {
        ITypeMapper GetTypeMapper(SqlEngineVersion version);
    }
}