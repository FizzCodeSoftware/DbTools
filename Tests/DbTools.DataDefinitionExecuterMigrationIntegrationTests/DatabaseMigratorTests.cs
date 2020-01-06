namespace FizzCode.DbTools.DataDefinitionExecuterMigrationIntegration.Tests
{
    using System.Linq;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Tests;
    using FizzCode.DbTools.DataDefinitionExecuter;
    using FizzCode.DbTools.DataDefinitionGenerator;
    using FizzCode.DbTools.DataDefinitionReader;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DatabaseMigratorTests : DataDefinitionExecuterMigrationIntegrationTests
    {
        [TestMethod]
        [SqlDialects]
        public void AddTableTest(SqlDialect sqlDialect)
        {
            // Create Test Db
            // Read up DD
            // Add a table
            // Detect changes (one new table)
            // Execute changes/migration

            var dd = new TestDatabaseSimple();
            _sqlExecuterTestAdapter.Check(sqlDialect);
            _sqlExecuterTestAdapter.InitializeAndCreate(sqlDialect.ToString(), dd);
            TestHelper.CheckFeature(sqlDialect, "ReadDdl");

            var databaseMigrator = new DatabaseMigrator(_sqlExecuterTestAdapter.GetExecuter(sqlDialect.ToString()),  SqlGeneratorFactory.CreateMigrationGenerator(sqlDialect, _sqlExecuterTestAdapter.GetContext(sqlDialect)));

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(sqlDialect, _sqlExecuterTestAdapter.GetExecuter(sqlDialect.ToString()));
            var db = ddlReader.GetDatabaseDefinition();

            var newTable = new SqlTable
            {
                SchemaAndTableName = "NewTableToMigrate"
            };
            newTable.AddInt32("Id", false).SetPK().SetIdentity();

            new PrimaryKeyNamingDefaultStrategy().SetPrimaryKeyName(newTable.Properties.OfType<PrimaryKey>().First());

            newTable.AddNVarChar("Name", 100);

            dd.AddTable(newTable);

            // databaseMigrator.NewTable(newTable);
        }
    }
}