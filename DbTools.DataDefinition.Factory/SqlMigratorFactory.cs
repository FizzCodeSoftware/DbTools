namespace FizzCode.DbTools.DataDefinition.Factory
{
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Factory.Interfaces;
    using FizzCode.DbTools.SqlExecuter;
    using FizzCode.LightWeight.AdoNet;

    public class SqlMigratorFactory : ISqlMigratorFactory
    {
        private readonly ISqlGeneratorFactory _sqlGeneratorFactory;
        private readonly ISqlExecuterFactory _sqlExecuterFactory;
        public SqlMigratorFactory(ISqlGeneratorFactory sqlGeneratorFactory, ISqlExecuterFactory sqlExecuterFactory)
        {
            _sqlGeneratorFactory = sqlGeneratorFactory;
            _sqlExecuterFactory = sqlExecuterFactory;
        }

        public DatabaseMigrator FromConnectionStringSettings(NamedConnectionString connectionString, Context context)
        {
            var sqlEngineVersion = connectionString.GetSqlEngineVersion();

            var migrationGenerator = _sqlGeneratorFactory.CreateMigrationGenerator(sqlEngineVersion, context);
            var executer = _sqlExecuterFactory.CreateSqlExecuter(connectionString, context);

            return new DatabaseMigrator(executer, migrationGenerator);
        }
    }
}