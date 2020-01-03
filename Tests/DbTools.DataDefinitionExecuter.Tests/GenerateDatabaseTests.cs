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
        [SqlDialects]
        public void GenerateForeignKeyCompositeTestDatabase(SqlDialect sqlDialect)
        {
            GenerateDatabase(new ForeignKeyCompositeTestsDb(), sqlDialect.ToString());
        }

        public static void GenerateDatabase(DatabaseDefinition dd, string connectionStringKey)
        {
            var connectionStringWithProvider = SetupAssemblyInitializer.ConnectionStrings[connectionStringKey];

            if (!TestHelper.ShouldRunIntegrationTest(connectionStringWithProvider.ProviderName))
                Assert.Inconclusive("Test is skipped, integration tests are not running.");

            var sqlDialect = SqlDialectHelper.GetSqlDialectFromProviderName(connectionStringWithProvider.ProviderName);

            TestHelper.CheckProvider(sqlDialect);

            var context = new GeneratorContext
            {
                Settings = TestHelper.GetDefaultTestSettings(sqlDialect),
                Logger = TestHelper.CreateLogger()
            };

            var databaseCreator = DatabaseCreator.FromConnectionStringSettings(dd, connectionStringWithProvider, context);

            try
            {
                databaseCreator.ReCreateDatabase(true);
            }
            finally
            {
                databaseCreator.CleanupDatabase();
            }
        }

        [TestMethod]
        [SqlDialects]
        public void GenerateDatabase_Index(SqlDialect sqlDialect)
        {
            GenerateDatabase(new IndexTestDb(), sqlDialect.ToString());
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