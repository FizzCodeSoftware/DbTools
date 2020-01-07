namespace FizzCode.DbTools.DataDefinitionExecuterMigrationIntegration.Tests
{
    using System.Linq;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Migration;
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

            /*if (sqlDialect == SqlDialect.MsSql)
                dd.DefaultSchema = "dbo";*/

            _sqlExecuterTestAdapter.Check(sqlDialect);
            _sqlExecuterTestAdapter.Initialize(sqlDialect.ToString(), dd);
            TestHelper.CheckFeature(sqlDialect, "ReadDdl");

            var databaseCreator = new DatabaseCreator(dd, _sqlExecuterTestAdapter.GetExecuter(sqlDialect.ToString()));

            databaseCreator.ReCreateDatabase(true);

            var databaseMigrator = new DatabaseMigrator(_sqlExecuterTestAdapter.GetExecuter(sqlDialect.ToString()),  SqlGeneratorFactory.CreateMigrationGenerator(sqlDialect, _sqlExecuterTestAdapter.GetContext(sqlDialect)));

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(sqlDialect, _sqlExecuterTestAdapter.GetExecuter(sqlDialect.ToString()));
            var ddInDatabase = ddlReader.GetDatabaseDefinition();

            var newTable = new SqlTable
            {
                SchemaAndTableName = "NewTableToMigrate"
            };
            newTable.AddInt32("Id", false).SetPK().SetIdentity();

            new PrimaryKeyNamingDefaultStrategy().SetPrimaryKeyName(newTable.Properties.OfType<PrimaryKey>().First());

            newTable.AddNVarChar("Name", 100);

            dd.AddTable(newTable);

            var comparer = new Comparer(_sqlExecuterTestAdapter.GetContext(sqlDialect));
            var changes = comparer.Compare(ddInDatabase, dd);

            var first = changes.First() as TableNew;
            Assert.AreEqual((SchemaAndTableName)"NewTableToMigrate", first.SchemaAndTableName);
        }
    }
}