namespace FizzCode.DbTools.DataDefinitionExecuter
{
    using System.Configuration;
    using System.Linq;
    using FizzCode.DbTools.Common;
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

        public static DatabaseCreator FromConnectionStringSettings(DatabaseDefinition databaseDefinition, ConnectionStringSettings connectionStringSettings, Settings settings)
        {
            var sqlDialect = SqlDialectHelper.GetSqlDialectFromConnectionStringSettings(connectionStringSettings);
            var generator = SqlGeneratorFactory.CreateGenerator(sqlDialect, settings);

            var executer = SqlExecuterFactory.CreateSqlExecuter(connectionStringSettings, generator);

            return new DatabaseCreator(databaseDefinition, executer);
        }

        public void ReCreateDatabase(bool createTables)
        {
            _executer.InitializeDatabase(true, DatabaseDefinition);

            if (createTables)
            {
                CreateTables();
            }
        }

        public void CreateTables()
        {
            CreateSchemas(DatabaseDefinition);

            foreach (var sqlTable in DatabaseDefinition.GetTables())
                CreateTable(sqlTable);

            foreach (var sqlTable in DatabaseDefinition.GetTables())
                CreateForeignkeys(sqlTable);

            foreach (var sqlTable in DatabaseDefinition.GetTables())
                CreateIndexes(sqlTable);

            foreach (var sqlTable in DatabaseDefinition.GetTables())
                CreateDbDescriptions(sqlTable);
        }

        private void CreateSchemas(DatabaseDefinition databaseDefinition)
        {
            foreach (var schemaName in databaseDefinition.GetSchemaNames())
            {
                var sql = _executer.Generator.CreateSchema(schemaName);
                _executer.ExecuteNonQuery(sql);
            }
        }

        public void CreateTable(SqlTable table)
        {
            var sql = _executer.Generator.CreateTable(table);
            _executer.ExecuteNonQuery(sql);
        }

        public void CreateForeignkeys(SqlTable table)
        {
            var sql = _executer.Generator.CreateForeignKeys(table);
            if (!string.IsNullOrEmpty(sql))
                _executer.ExecuteNonQuery(sql);
        }

        public void CreateIndexes(SqlTable table)
        {
            var sql = _executer.Generator.CreateIndexes(table);
            if (!string.IsNullOrEmpty(sql))
                _executer.ExecuteNonQuery(sql);
        }

        public void CreateDbDescriptions(SqlTable table)
        {
            var sqlStatementWithParameters = _executer.Generator.CreateDbTableDescription(table);
            if (sqlStatementWithParameters != null)
                _executer.ExecuteNonQuery(sqlStatementWithParameters);

            foreach (var column in table.Columns.Values)
            {
                sqlStatementWithParameters = _executer.Generator.CreateDbColumnDescription(column);
                if (sqlStatementWithParameters != null)
                    _executer.ExecuteNonQuery(sqlStatementWithParameters);
            }
        }

        public void DropAllViews()
        {
            var sql = _executer.Generator.DropAllViews();
            _executer.ExecuteNonQuery(sql);
        }

        public void DropAllForeignKeys()
        {
            var sql = _executer.Generator.DropAllForeignKeys();
            _executer.ExecuteNonQuery(sql);
        }

        public void DropAllTables()
        {
            var sql = _executer.Generator.DropAllTables();
            _executer.ExecuteNonQuery(sql);
        }

        public void DropAllSchemas()
        {
            var schemaNames = DatabaseDefinition
                .GetSchemaNames()
                .ToList();

            var sql = _executer.Generator.DropSchemas(schemaNames);
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