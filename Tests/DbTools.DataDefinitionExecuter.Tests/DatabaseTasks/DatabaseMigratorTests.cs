namespace FizzCode.DbTools.DataDefinitionExecuter.Tests
{
    using System.Linq;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Migration;
    using FizzCode.DbTools.DataDefinition.Tests;
    using FizzCode.DbTools.DataDefinitionGenerator;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DatabaseMigratorTests
    {
        [TestMethod]
        [SqlDialects]
        public void NewTableTest(SqlDialect sqlDialect)
        {
            var connectionStringWithProvider = SetupAssemblyInitializer.ConnectionStrings[sqlDialect.ToString()];

            if (!TestHelper.ShouldRunIntegrationTest(connectionStringWithProvider.ProviderName))
                Assert.Inconclusive("Test is skipped, integration tests are not running.");

            TestHelper.CheckProvider(sqlDialect);

            var context = new GeneratorContext
            {
                Settings = TestHelper.GetDefaultTestSettings(sqlDialect),
                Logger = TestHelper.CreateLogger()
            };

            var databaseMigrator = DatabaseMigrator.FromConnectionStringSettings(connectionStringWithProvider, context);

            var databaseCreator = DatabaseCreator.FromConnectionStringSettings(new TestDatabaseSimple(), connectionStringWithProvider, context);

            try
            {
                databaseCreator.ReCreateDatabase(true);

                var tableNew = new TableNew
                {
                    SchemaAndTableName = "NewTableToMigrate"
                };
                ((SqlTable)tableNew).AddInt32("Id", false).SetPK().SetIdentity();

                new PrimaryKeyNamingDefaultStrategy().SetPrimaryKeyName(tableNew.Properties.OfType<PrimaryKey>().First());

                ((SqlTable)tableNew).AddNVarChar("Name", 100);

                databaseMigrator.NewTable(tableNew);
            }
            finally
            {
                databaseCreator.CleanupDatabase();
            }
        }
    }
}