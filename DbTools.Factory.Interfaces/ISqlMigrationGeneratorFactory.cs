using FizzCode.DbTools.Interfaces;

namespace FizzCode.DbTools.Factory.Interfaces;
public interface ISqlMigrationGeneratorFactory
{
    ISqlMigrationGenerator CreateMigrationGenerator(SqlEngineVersion version);
}