namespace FizzCode.DbTools.DataDefinitionExecuter.Tests
{
    using System;
    using System.Configuration;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Tests;
    using FizzCode.DbTools.DataDefinitionExecuter;
    using FizzCode.DbTools.DataDefinitionGenerator;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class GenerateDatabaseTests
    {
        [TestMethod]
        [SqlDialects]
        public void GenerateTestDatabaseSimple(SqlDialect sqlDialect)
        {
            GenerateDatabase(new TestDatabaseSimple(), sqlDialect.ToString());
        }

        [TestMethod]
        public void GenerateForeignKeyCompositeTestDatabase()
        {
            GenerateDatabase(new ForeignKeyCompositeTestsDb(), "MsSql");
        }

        public void GenerateDatabase(DatabaseDefinition dd, string connectionStringKey, bool isIntegrationTest = true)
        {
            if (isIntegrationTest && !TestHelper.ShouldForceIntegrationTests())
                Assert.Inconclusive("Test is skipped, integration tests are not running.");

            var connectionStringSettings = ConfigurationManager.ConnectionStrings[connectionStringKey];

            var sqlDialect = SqlDialectHelper.GetSqlDialectFromConnectionStringSettings(connectionStringSettings);

            var generateForeignKeyCompositeTestDatabase = DatabaseCreator.FromConnectionStringSettings(dd, connectionStringSettings, TestHelper.GetDefaultTestSettings(sqlDialect));
            try
            {
                generateForeignKeyCompositeTestDatabase.ReCreateDatabase(true);
            }
            finally
            {
                var generator = SqlGeneratorFactory.CreateGenerator(SqlDialectHelper.GetSqlDialectFromConnectionStringSettings(connectionStringSettings), TestHelper.GetDefaultTestSettings(sqlDialect));

                var executer = SqlExecuterFactory.CreateSqlExecuter(connectionStringSettings, generator);
                executer.CleanupDatabase(dd);
            }
        }

        [TestMethod]
        public void GenerateDatabase_Index()
        {
            GenerateDatabase(new IndexTestDb(), "MsSql");
        }

        [TestMethod]
        public void GenerateDatabase_TableDescription()
        {
            GenerateDatabase(new TableDescriptionTestDb(), "MsSql");
        }

        [TestMethod]
        public void GenerateDatabase_ColumnDescription()
        {
            GenerateDatabase(new ColumnDescriptionTestDb(), "MsSql");
        }

        public class IndexTestDb : DatabaseDeclaration
        {
            public static LazySqlTable Table = new LazySqlTable(() =>
            {
                var table = new SqlTableDeclaration();
                table.AddInt32("Id").SetPK().SetIdentity();
                table.AddNVarChar("Name", 100);
                table.AddIndex("Name");
                table.AddIndex("Id", "Name");
                return table;
            });
        }

        public class TableDescriptionTestDb : DatabaseDeclaration
        {
            public static LazySqlTable Table = new LazySqlTable(() =>
            {
                var table = new SqlTableDeclaration();
                table.AddInt32("Id").SetPK().SetIdentity();
                table.AddNVarChar("Name", 100);
                table.AddDescription("Table description");
                return table;
            });
        }

        public class ColumnDescriptionTestDb : DatabaseDeclaration
        {
            public static LazySqlTable Table = new LazySqlTable(() =>
            {
                var table = new SqlTableDeclaration();
                table.AddInt32("Id").SetPK().SetIdentity().AddDescription("Id Column description");
                table.AddNVarChar("Name", 100).AddDescription("Name Column description");
                return table;
            });
        }

        [TestMethod]
        public void GenerateDatabase_DefaultValue()
        {
            GenerateDatabase(new DefaultValueTestDb(), "MsSql");
        }

        public class DefaultValueTestDb : DatabaseDeclaration
        {
            public static LazySqlTable Table = new LazySqlTable(() =>
            {
                var table = new SqlTableDeclaration();
                table.AddInt32("Id").SetPK().SetIdentity();
                table.AddNVarChar("Name", 100).AddDefaultValue("'apple'");
                table.AddDateTime("DateTime").AddDefaultValue("'" + new DateTime(2019,8,7,13,59,57,357).ToString("yyyy-M-d HH:mm:ss.fff") + "'");
                return table;
            });
        }

        [TestMethod]
        public void DatabaseDefinitionWithSchemaTableNameSeparator()
        {
            GenerateDatabase(new SchemaTableNameSeparatorTestDb(), "MsSql");
        }

        [TestMethod]
        public void DatabaseDefinitionWithSchemaAndDefaultSchema()
        {
            GenerateDatabase(new SchemaTableNameDefaultSchemaTestDb(), "MsSql");
        }

        public class SchemaTableNameSeparatorTestDb : DatabaseDeclaration
        {
            public static LazySqlTable SchemaAꜗTable = new LazySqlTable(() =>
            {
                var table = new SqlTableDeclaration();
                table.AddInt32("Id").SetPKIdentity();
                table.AddNVarChar("Name", 100);
                return table;
            });

            public static LazySqlTable SchemaBꜗTable = new LazySqlTable(() =>
            {
                var table = new SqlTableDeclaration();
                table.AddInt32("Id").SetPKIdentity();
                table.AddNVarChar("Name", 100);
                table.AddForeignKey(SchemaAꜗTable);
                return table;
            });
        }

        public class SchemaTableNameDefaultSchemaTestDb : SchemaTableNameSeparatorTestDb
        {
            public static LazySqlTable Table = new LazySqlTable(() =>
            {
                var table = new SqlTableDeclaration();
                table.AddInt32("Id").SetPKIdentity();
                table.AddNVarChar("Name", 100);
                return table;
            });
        }
    }
}
