namespace FizzCode.DbTools.DataDefinition.Base.Interfaces
{
    public interface ITypeMapperFactory
    {
        ITypeMapper GetTypeMapper(SqlEngineVersion version);
    }
}