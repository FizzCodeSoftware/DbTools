namespace FizzCode.DbTools.DataDefinition.Factory
{
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Factory.Interfaces;
    using FizzCode.DbTools.SqlExecuter;
    using FizzCode.LightWeight.AdoNet;

    public class DatabaseCreatorFactory
    {
        private readonly ISqlExecuterFactory _sqlExecuterFactory;
        public DatabaseCreatorFactory(ISqlExecuterFactory sqlExecuterFactory)
        {
            _sqlExecuterFactory = sqlExecuterFactory;

        }

        public DatabaseCreator FromConnectionStringSettings(DatabaseDefinition databaseDefinition, NamedConnectionString connectionString, Context context)
        {
            var executer = _sqlExecuterFactory.CreateSqlExecuter(connectionString, context);
            return new DatabaseCreator(databaseDefinition, executer);
        }
    }
}