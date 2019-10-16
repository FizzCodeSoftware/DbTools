namespace FizzCode.DbTools.DataDefinitionExecuter.Tests
{
    using System;
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
            var connectionStringWithProvider = SetupAssemblyInitializer.ConnectionStrings[connectionStringKey];

            if (isIntegrationTest && !TestHelper.ShouldRunIntegrationTest(connectionStringWithProvider.ProviderName))
                Assert.Inconclusive("Test is skipped, integration tests are not running.");

            var sqlDialect = SqlDialectHelper.GetSqlDialectFromProviderName(connectionStringWithProvider.ProviderName);

            TestHelper.CheckProvider(sqlDialect);

            var databaseCreator = DatabaseCreator.FromConnectionStringSettings(dd, connectionStringWithProvider, TestHelper.GetDefaultTestSettings(sqlDialect));

            try
            {
                databaseCreator.ReCreateDatabase(true);
            }
            finally
            {
                var generator = SqlGeneratorFactory.CreateGenerator(SqlDialectHelper.GetSqlDialectFromProviderName(connectionStringWithProvider.ProviderName), TestHelper.GetDefaultTestSettings(sqlDialect));

                var executer = SqlExecuterFactory.CreateSqlExecuter(connectionStringWithProvider, generator);
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
            public SqlTable Table { get; } = AddTable(table =>
            {
                table.AddInt32("Id").SetPK().SetIdentity();
                table.AddNVarChar("Name", 100);
                table.AddIndex("Name");
                table.AddIndex("Id", "Name");
            });
        }

        public class TableDescriptionTestDb : DatabaseDeclaration
        {
            public SqlTable Table { get; } = AddTable(table =>
            {
                table.AddInt32("Id").SetPK().SetIdentity();
                table.AddNVarChar("Name", 100);
                table.AddDescription("Table description");
            });
        }

        public class ColumnDescriptionTestDb : DatabaseDeclaration
        {
            public SqlTable Table { get; } = AddTable(table =>
            {
                table.AddInt32("Id").SetPK().SetIdentity().AddDescription("Id Column description");
                table.AddNVarChar("Name", 100).AddDescription("Name Column description");
            });
        }

        [TestMethod]
        public void GenerateDatabase_DefaultValue()
        {
            GenerateDatabase(new DefaultValueTestDb(), "MsSql");
        }

        public class DefaultValueTestDb : DatabaseDeclaration
        {
            public SqlTable Table { get; } = AddTable(table =>
            {
                table.AddInt32("Id").SetPK().SetIdentity();
                table.AddNVarChar("Name", 100).AddDefaultValue("'apple'");
                table.AddDateTime("DateTime").AddDefaultValue("'" + new DateTime(2019, 8, 7, 13, 59, 57, 357).ToString("yyyy-M-d HH:mm:ss.fff") + "'");
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
            public SqlTable SchemaAꜗTable { get; } = AddTable(table =>
            {
                table.AddInt32("Id").SetPK().SetIdentity();
                table.AddNVarChar("Name", 100);
            });

            public SqlTable SchemaBꜗTable { get; } = AddTable(table =>
            {
                table.AddInt32("Id").SetPK().SetIdentity();
                table.AddNVarChar("Name", 100);
                table.AddForeignKey(nameof(SchemaAꜗTable));
            });
        }

        public class SchemaTableNameDefaultSchemaTestDb : SchemaTableNameSeparatorTestDb
        {
            public SqlTable Table { get; } = AddTable(table =>
            {
                table.AddInt32("Id").SetPK().SetIdentity();
                table.AddNVarChar("Name", 100);
            });
        }
    }
}