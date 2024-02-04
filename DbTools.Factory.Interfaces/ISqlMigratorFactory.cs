using FizzCode.DbTools.Interfaces;
using FizzCode.LightWeight.AdoNet;

namespace FizzCode.DbTools.Factory.Interfaces;
public interface ISqlMigratorFactory
{
    IDatabaseMigrator FromConnectionStringSettings(NamedConnectionString connectionString);
}