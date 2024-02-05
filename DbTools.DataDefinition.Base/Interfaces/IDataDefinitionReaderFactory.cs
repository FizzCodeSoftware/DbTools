using FizzCode.LightWeight;

namespace FizzCode.DbTools.DataDefinition.Base.Interfaces;
public interface IDataDefinitionReaderFactory
{
    IDataDefinitionReader CreateDataDefinitionReader(NamedConnectionString connectionString, ISchemaNamesToRead schemaNames);
}