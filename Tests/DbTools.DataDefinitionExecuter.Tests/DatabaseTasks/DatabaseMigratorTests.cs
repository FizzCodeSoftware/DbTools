namespace FizzCode.DbTools.DataDefinition.SqlExecuter.Tests
{
    using System.Linq;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Generic1;
    using FizzCode.DbTools.DataDefinition.Migration;
    using FizzCode.DbTools.DataDefinition.Tests;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DatabaseMigratorTests : SqlExecuterTestsBase
    {
        [TestMethod]
        [LatestSqlVersions]
        public void NewTableTest(SqlEngineVersion version)
        {
            SqlExecuterTestAdapter.Check(version);
            var dd = new TestDatabaseSimple();
            SqlExecuterTestAdapter.InitializeAndCreate(version.UniqueName, dd);

            var context = new Context
            {
                Settings = TestHelper.GetDefaultTestSettings(version),
                Logger = TestHelper.CreateLogger()
            };

            var migrationGenerator = SqlGeneratorFactory.CreateMigrationGenerator(version, context);

            var executer = SqlExecuterTestAdapter.GetExecuter(version.UniqueName);

            var databaseMigrator = new DatabaseMigrator(executer, migrationGenerator);
            var tableNew = new TableNew
            {
                SchemaAndTableName = "NewTableToMigrate"
            };
            tableNew.AddInt32("Id", false).SetPK().SetIdentity();

            new PrimaryKeyNamingDefaultStrategy().SetPrimaryKeyName(tableNew.Properties.OfType<PrimaryKey>().First());

            ((SqlTable)tableNew).AddNVarChar("Name", 100);

            dd.AddTable(tableNew);

            databaseMigrator.NewTable(tableNew);
        }
    }
}