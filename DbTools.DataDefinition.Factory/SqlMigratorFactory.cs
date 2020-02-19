namespace FizzCode.DbTools.DataDefinition
{
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition.SqlExecuter;

    public static class SqlMigratorFactory
    {
        public static DatabaseMigrator FromConnectionStringSettings(ConnectionStringWithProvider connectionStringWithProvider, Context context)
        {
            var generator = SqlGeneratorFactory.CreateGenerator(connectionStringWithProvider.SqlEngineVersion, context);
            var migrationGenerator = SqlGeneratorFactory.CreateMigrationGenerator(connectionStringWithProvider.SqlEngineVersion, context);
            var executer = SqlExecuterFactory.CreateSqlExecuter(connectionStringWithProvider, generator);

            return new DatabaseMigrator(executer, migrationGenerator);
        }
    }
}