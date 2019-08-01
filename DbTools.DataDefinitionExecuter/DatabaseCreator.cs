namespace FizzCode.DbTools.DataDefinitionExecuter
{
    using System.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionGenerator;

    public class DatabaseCreator
    {
        private readonly SqlExecuter _executer;
        public DatabaseDefinition DatabaseDefinition { get; }

        public DatabaseCreator(DatabaseDefinition databaseDefinition, SqlExecuter sqlExecuter)
        {
            DatabaseDefinition = databaseDefinition;
            _executer = sqlExecuter;
        }

        public static DatabaseCreator FromConnectionStringSettings(DatabaseDefinition databaseDefinition, ConnectionStringSettings connectionStringSettings)
        {
            var sqlDialect = SqlDialectHelper.GetSqlDialectFromConnectionStringSettings(connectionStringSettings);
            var generator = SqlGeneratorFactory.CreateGenerator(sqlDialect);
            var executer = SqlExecuterFactory.CreateSqlExecuter(connectionStringSettings, generator);

            return new DatabaseCreator(databaseDefinition, executer);
        }

        public void ReCreateDatabase(bool createTables)
        {
            _executer.DropDatabaseIfExists();
            _executer.CreateDatabase(false);

            if (createTables)
            {
                foreach (var sqlTable in DatabaseDefinition.GetTables())
                {
                    CreateTable(sqlTable);
                }

                foreach (var sqlTable in DatabaseDefinition.GetTables())
                {
                    CreateFKIndexConstraints(sqlTable);
                }
            }
        }

        public void CreateTable(SqlTable table)
        {
            var sql = _executer.Generator.CreateTable(table);
            _executer.ExecuteNonQuery(sql);
        }

        public void CreateFKIndexConstraints(SqlTable table)
        {
            var sql = _executer.Generator.CreateForeignKey(table);
            if (!string.IsNullOrEmpty(sql))
                _executer.ExecuteNonQuery(sql);
        }

        public void DropAllTables()
        {
            var sql = _executer.Generator.DropAllTables();
            _executer.ExecuteNonQuery(sql);
        }

        public bool IsTableExists(SqlTable table)
        {
            var sql = _executer.Generator.TableExists(table);
            var zeroOrOne = (int)_executer.ExecuteScalar(sql);

            return zeroOrOne == 1;
        }

        public bool IsTableEmpty(SqlTable table)
        {
            var sql = _executer.Generator.TableNotEmpty(table);
            var zeroOrOne = (int)_executer.ExecuteScalar(sql);
            return zeroOrOne == 0;
        }
    }
}