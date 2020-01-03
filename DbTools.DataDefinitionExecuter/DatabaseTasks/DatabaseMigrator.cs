namespace FizzCode.DbTools.DataDefinitionExecuter
{
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Migration;
    using FizzCode.DbTools.DataDefinitionGenerator;

    public class DatabaseMigrator : DatabaseTask
    {
        public DatabaseMigrator(SqlExecuter sqlExecuter, ISqlMigrationGenerator migrationGenerator) : base(sqlExecuter)
        {
            MigrationGenerator = migrationGenerator;
        }

        protected ISqlMigrationGenerator MigrationGenerator { get; }

        public static DatabaseMigrator FromConnectionStringSettings(ConnectionStringWithProvider connectionStringWithProvider, GeneratorContext context)
        {
            var sqlDialect = SqlDialectHelper.GetSqlDialectFromProviderName(connectionStringWithProvider.ProviderName);

            var generator = SqlGeneratorFactory.CreateGenerator(sqlDialect, context);
            var migrationGenerator = SqlGeneratorFactory.CreateMigrationGenerator(sqlDialect, context);

            var executer = SqlExecuterFactory.CreateSqlExecuter(connectionStringWithProvider, generator);

            return new DatabaseMigrator(executer, migrationGenerator);
        }

        public void NewTable(TableNew tableNew)
        {
            var sql = MigrationGenerator.CreateTable(tableNew);
            Executer.ExecuteNonQueryMaster(sql);
        }

        public void DeleteTable(TableDelete tableDelete)
        {
            var sql = MigrationGenerator.DropTable(tableDelete);
            Executer.ExecuteNonQueryMaster(sql);
        }
    }
}