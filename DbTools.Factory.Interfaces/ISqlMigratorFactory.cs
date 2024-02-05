using FizzCode.DbTools.Interfaces;
using FizzCode.LightWeight;

namespace FizzCode.DbTools.Factory.Interfaces;
public interface ISqlMigratorFactory
{
    IDatabaseMigrator FromConnectionStringSettings(NamedConnectionString connectionString);
}