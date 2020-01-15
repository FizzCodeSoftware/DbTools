namespace FizzCode.DbTools.DataDefinitionExecuter
{
    using System.Linq;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionGenerator;

    public class DatabaseCreator : DatabaseTask
    {
        public DatabaseDefinition DatabaseDefinition { get; }

        public DatabaseCreator(DatabaseDefinition databaseDefinition, SqlExecuter sqlExecuter) : base(sqlExecuter)
        {
            DatabaseDefinition = databaseDefinition;
        }

        public static DatabaseCreator FromConnectionStringSettings(DatabaseDefinition databaseDefinition, ConnectionStringWithProvider connectionStringWithProvider, Context context)
        {
            var sqlDialect = SqlDialectHelper.GetSqlDialectFromProviderName(connectionStringWithProvider.ProviderName);

            // TODO version detection?
            var version = SqlEngines.GetLatestVersion(sqlDialect);

            var generator = SqlGeneratorFactory.CreateGenerator(version, context);

            var executer = SqlExecuterFactory.CreateSqlExecuter(connectionStringWithProvider, generator);

            return new DatabaseCreator(databaseDefinition, executer);
        }

        public void ReCreateDatabase(bool createTables)
        {
            Executer.InitializeDatabase(true, DatabaseDefinition);

            if (createTables)
            {
                CreateTables();
            }
        }

        public void CreateTables()
        {
            CreateSchemas(DatabaseDefinition);

            var tables = DatabaseDefinition.GetTables();

            foreach (var sqlTable in tables)
                CreateTable(sqlTable);

            foreach (var sqlTable in tables)
                CreateForeignkeys(sqlTable);

            foreach (var sqlTable in tables)
                CreateIndexes(sqlTable);

            foreach (var sqlTable in tables)
                CreateDbDescriptions(sqlTable);
        }

        private void CreateSchemas(DatabaseDefinition databaseDefinition)
        {
            foreach (var schemaName in databaseDefinition.GetSchemaNames())
            {
                if (schemaName == Executer.Generator.Context.Settings.SqlDialectSpecificSettings.GetAs<string>("DefaultSchema", null))
                    continue;

                var sql = Executer.Generator.CreateSchema(schemaName);
                Executer.ExecuteNonQuery(sql);
            }
        }

        public void CreateTable(SqlTable table)
        {
            var sql = Executer.Generator.CreateTable(table);
            Executer.ExecuteNonQuery(sql);
        }

        public void CreateForeignkeys(SqlTable table)
        {
            var sql = Executer.Generator.CreateForeignKeys(table);
            if (!string.IsNullOrEmpty(sql))
                Executer.ExecuteNonQuery(sql);
        }

        public void CreateIndexes(SqlTable table)
        {
            var sql = Executer.Generator.CreateIndexes(table);
            if (!string.IsNullOrEmpty(sql))
                Executer.ExecuteNonQuery(sql);
        }

        public void CreateDbDescriptions(SqlTable table)
        {
            var sqlStatementWithParameters = Executer.Generator.CreateDbTableDescription(table);
            if (sqlStatementWithParameters != null)
                Executer.ExecuteNonQuery(sqlStatementWithParameters);

            foreach (var column in table.Columns)
            {
                sqlStatementWithParameters = Executer.Generator.CreateDbColumnDescription(column);
                if (sqlStatementWithParameters != null)
                    Executer.ExecuteNonQuery(sqlStatementWithParameters);
            }
        }

        public void DropAllViews()
        {
            var sql = Executer.Generator.DropAllViews();
            Executer.ExecuteNonQuery(sql);
        }

        public void DropAllForeignKeys()
        {
            var sql = Executer.Generator.DropAllForeignKeys();
            Executer.ExecuteNonQuery(sql);
        }

        public void DropAllTables()
        {
            var sql = Executer.Generator.DropAllTables();
            Executer.ExecuteNonQuery(sql);
        }

        public void DropAllSchemas()
        {
            var schemaNames = DatabaseDefinition
                .GetSchemaNames()
                .ToList();

            var sql = Executer.Generator.DropSchemas(schemaNames);
            Executer.ExecuteNonQuery(sql);
        }

        public bool IsTableExists(SqlTable table)
        {
            var sql = Executer.Generator.TableExists(table);
            var zeroOrOne = (int)Executer.ExecuteScalar(sql);

            return zeroOrOne == 1;
        }

        public bool IsTableEmpty(SqlTable table)
        {
            var sql = Executer.Generator.TableNotEmpty(table);
            var zeroOrOne = (int)Executer.ExecuteScalar(sql);
            return zeroOrOne == 0;
        }

        public void CleanupDatabase()
        {
            Executer.CleanupDatabase();
        }
    }
}