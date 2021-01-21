namespace FizzCode.DbTools.DataDefinition
{
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition.SqlExecuter;
    using FizzCode.LightWeight.AdoNet;

    public static class SqlMigratorFactory
    {
        public static DatabaseMigrator FromConnectionStringSettings(NamedConnectionString connectionString, Context context)
        {
            var sqlEngineVersion = connectionString.GetSqlEngineVersion();

            var generator = SqlGeneratorFactory.CreateGenerator(sqlEngineVersion, context);
            var migrationGenerator = SqlGeneratorFactory.CreateMigrationGenerator(sqlEngineVersion, context);
            var executer = SqlExecuterFactory.CreateSqlExecuter(connectionString, generator);

            return new DatabaseMigrator(executer, migrationGenerator);
        }
    }
}