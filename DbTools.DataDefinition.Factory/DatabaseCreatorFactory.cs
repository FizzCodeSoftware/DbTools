namespace FizzCode.DbTools.DataDefinition
{
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.SqlExecuter;
    using FizzCode.LightWeight.AdoNet;

    public static class DatabaseCreatorFactory
    {
        public static DatabaseCreator FromConnectionStringSettings(DatabaseDefinition databaseDefinition, NamedConnectionString connectionString, Context context)
        {
            var generator = SqlGeneratorFactory.CreateGenerator(connectionString.GetSqlEngineVersion(), context);
            var executer = SqlExecuterFactory.CreateSqlExecuter(connectionString, generator);
            return new DatabaseCreator(databaseDefinition, executer);
        }
    }
}