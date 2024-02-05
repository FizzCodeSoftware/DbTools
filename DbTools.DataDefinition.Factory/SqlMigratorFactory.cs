using FizzCode.DbTools.Factory.Interfaces;
using FizzCode.DbTools.Interfaces;
using FizzCode.DbTools.SqlExecuter;
using FizzCode.LightWeight;

namespace FizzCode.DbTools.DataDefinition.Factory;
public class SqlMigratorFactory : ISqlMigratorFactory
{
    private readonly ISqlMigrationGeneratorFactory _sqlMigrationGeneratorFactory;
    private readonly ISqlExecuterFactory _sqlExecuterFactory;

    public SqlMigratorFactory(ISqlMigrationGeneratorFactory sqlMigrationGeneratorFactory, ISqlExecuterFactory sqlExecuterFactory)
    {
        _sqlMigrationGeneratorFactory = sqlMigrationGeneratorFactory;
        _sqlExecuterFactory = sqlExecuterFactory;
    }

    public IDatabaseMigrator FromConnectionStringSettings(NamedConnectionString connectionString)
    {
        var sqlEngineVersion = connectionString.GetSqlEngineVersion();

        var migrationGenerator = _sqlMigrationGeneratorFactory.CreateMigrationGenerator(sqlEngineVersion);
        var executer = _sqlExecuterFactory.CreateSqlExecuter(connectionString);

        return new DatabaseMigrator(executer, migrationGenerator);
    }
}