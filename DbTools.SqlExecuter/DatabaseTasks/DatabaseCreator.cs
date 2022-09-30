namespace FizzCode.DbTools.SqlExecuter
{
    using System.Linq;
    using FizzCode.DbTools.DataDefinition.Base;
    using FizzCode.DbTools.Interfaces;

    public class DatabaseCreator : DatabaseTask
    {
        public IDatabaseDefinition DatabaseDefinition { get; }

        public DatabaseCreator(IDatabaseDefinition databaseDefinition, ISqlStatementExecuter sqlExecuter)
            : base(sqlExecuter)
        {
            DatabaseDefinition = databaseDefinition;
        }

        public void ReCreateDatabase(bool createElements)
        {
            Executer.InitializeDatabase(true, DatabaseDefinition);

            if (createElements)
            {
                CreateTables();
                CreateStoredProcedures();
                CreateViews();
            }
        }

        public void CreateTables()
        {
            CreateSchemas(DatabaseDefinition);

            var tables = DatabaseDefinition.GetTables();

            foreach (var sqlTable in tables)
                CreateTable(sqlTable);

            foreach (var sqlTable in tables)
                CreateIndexes(sqlTable);

            foreach (var sqlTable in tables)
                CreateUniqueConstraints(sqlTable);

            foreach (var sqlTable in tables)
                CreateForeignkeys(sqlTable);

            foreach (var sqlTable in tables)
                CreateDbDescriptions(sqlTable);
        }

        private void CreateSchemas(IDatabaseDefinition databaseDefinition)
        {
            foreach (var schemaName in databaseDefinition.GetSchemaNames())
            {
                if (schemaName == Executer.Generator.Context.Settings.SqlVersionSpecificSettings.GetAs<string>("DefaultSchema", null))
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

        public void CreateUniqueConstraints(SqlTable table)
        {
            var sql = Executer.Generator.CreateUniqueConstrainsts(table);
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

        public void CreateStoredProcedures()
        {
            foreach (var sp in DatabaseDefinition.StoredProcedures)
            {
                var sql = Executer.Generator.CreateStoredProcedure(sp);
                Executer.ExecuteNonQuery(sql);
            }
        }

        public void CreateViews()
        {
            foreach (var view in DatabaseDefinition.GetViews())
            {
                var sql = Executer.Generator.CreateView(view);
                Executer.ExecuteNonQuery(sql);
            }
        }
    }
}