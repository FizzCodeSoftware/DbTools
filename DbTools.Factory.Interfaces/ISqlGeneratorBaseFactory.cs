namespace FizzCode.DbTools.Factory.Interfaces
{
    using FizzCode.DbTools;
    using FizzCode.DbTools.Interfaces;

    public interface ISqlGeneratorBaseFactory
    {
        ISqlGeneratorBase CreateGenerator(SqlEngineVersion version);
    }
}