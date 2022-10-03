namespace FizzCode.DbTools.Factory.Interfaces
{
    using FizzCode.DbTools;
    using FizzCode.DbTools.Interfaces;

    public interface ISqlMigrationGeneratorFactory
    {
        ISqlMigrationGenerator CreateMigrationGenerator(SqlEngineVersion version);
    }
}