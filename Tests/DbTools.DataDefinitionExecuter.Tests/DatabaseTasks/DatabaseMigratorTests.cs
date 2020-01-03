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

            var generator = SqlGeneratorFactory.CreateGenerator(sqlDialect, context);
            var migrationGenerator = SqlGeneratorFactory.CreateMigrationGenerator(sqlDialect, context);

            var executer = SqlExecuterFactory.CreateSqlExecuter(connectionStringWithProvider, generator);

            var databaseMigrator = new DatabaseMigrator(executer, migrationGenerator);

            var databaseCreator = new DatabaseCreator(new TestDatabaseSimple(), executer);

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