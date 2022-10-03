namespace FizzCode.DbTools.DataDefinition.Base.Interfaces
{
    using FizzCode.LightWeight.AdoNet;

    public interface IDataDefinitionReaderFactory
    {
        IDataDefinitionReader CreateDataDefinitionReader(NamedConnectionString connectionString, ISchemaNamesToRead schemaNames);
    }
}