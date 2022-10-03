namespace FizzCode.DbTools.Factory.Interfaces
{
    using FizzCode.DbTools;
    using FizzCode.DbTools.Interfaces;

    public interface ISqlGeneratorFactory
    {
        ISqlGenerator CreateSqlGenerator(SqlEngineVersion version);
    }
}