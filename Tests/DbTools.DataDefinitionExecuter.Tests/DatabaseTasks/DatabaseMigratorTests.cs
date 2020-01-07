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
    public class DatabaseMigratorTests : DataDefinitionExecuterTests
    {
        [TestMethod]
        [SqlDialects]
        public void NewTableTest(SqlDialect sqlDialect)
        {
            _sqlExecuterTestAdapter.Check(sqlDialect);
            var dd = new TestDatabaseSimple();
            _sqlExecuterTestAdapter.InitializeAndCreate(sqlDialect.ToString(), dd);

            var context = new Context
            {
                Settings = TestHelper.GetDefaultTestSettings(sqlDialect),
                Logger = TestHelper.CreateLogger()
            };

            var migrationGenerator = SqlGeneratorFactory.CreateMigrationGenerator(sqlDialect, context);

            var executer = _sqlExecuterTestAdapter.GetExecuter(sqlDialect.ToString());

            var databaseMigrator = new DatabaseMigrator(executer, migrationGenerator);
            var tableNew = new TableNew

            {
                SchemaAndTableName = "NewTableToMigrate"
            };
            ((SqlTable)tableNew).AddInt32("Id", false).SetPK().SetIdentity();

            new PrimaryKeyNamingDefaultStrategy().SetPrimaryKeyName(tableNew.Properties.OfType<PrimaryKey>().First());

            ((SqlTable)tableNew).AddNVarChar("Name", 100);

            databaseMigrator.NewTable(tableNew);
        }
    }
}