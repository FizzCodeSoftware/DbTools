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

            Init(sqlDialect, dd);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(sqlDialect, _sqlExecuterTestAdapter.GetExecuter(sqlDialect.ToString()));
            var ddInDatabase = ddlReader.GetDatabaseDefinition();

            AddTable(dd);

            var comparer = new Comparer(_sqlExecuterTestAdapter.GetContext(sqlDialect));
            var changes = comparer.Compare(ddInDatabase, dd);

            var first = changes.First() as TableNew;
            Assert.AreEqual((SchemaAndTableName)"NewTableToMigrate", first.SchemaAndTableName);

            var databaseMigrator = new DatabaseMigrator(_sqlExecuterTestAdapter.GetExecuter(sqlDialect.ToString()), SqlGeneratorFactory.CreateMigrationGenerator(sqlDialect, _sqlExecuterTestAdapter.GetContext(sqlDialect)));

            databaseMigrator.NewTable(first);
        }

        private static void Init(SqlDialect sqlDialect, TestDatabaseSimple dd)
        {
            _sqlExecuterTestAdapter.Check(sqlDialect);
            _sqlExecuterTestAdapter.Initialize(sqlDialect.ToString(), dd);
            TestHelper.CheckFeature(sqlDialect, "ReadDdl");

            var databaseCreator = new DatabaseCreator(dd, _sqlExecuterTestAdapter.GetExecuter(sqlDialect.ToString()));

            databaseCreator.ReCreateDatabase(true);
        }

        private static void AddTable(TestDatabaseSimple dd)
        {
            var newTable = new SqlTable
            {
                SchemaAndTableName = "NewTableToMigrate"
            };
            newTable.AddInt32("Id", false).SetPK().SetIdentity();

            new PrimaryKeyNamingDefaultStrategy().SetPrimaryKeyName(newTable.Properties.OfType<PrimaryKey>().First());

            newTable.AddNVarChar("Name", 100);

            dd.AddTable(newTable);
        }

        [TestMethod]
        [SqlDialects]
        public void RemoveTableTest(SqlDialect sqlDialect)
        {
            var dd = new TestDatabaseSimple();
            AddTable(dd);
            Init(sqlDialect, dd);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(sqlDialect, _sqlExecuterTestAdapter.GetExecuter(sqlDialect.ToString()));
            var ddInDatabase = ddlReader.GetDatabaseDefinition();

            var comparer = new Comparer(_sqlExecuterTestAdapter.GetContext(sqlDialect));
            var changes = comparer.Compare(ddInDatabase, new TestDatabaseSimple());

            var first = changes.First() as TableDelete;
            Assert.AreEqual((SchemaAndTableName)"NewTableToMigrate", first.SchemaAndTableName);

            var databaseMigrator = new DatabaseMigrator(_sqlExecuterTestAdapter.GetExecuter(sqlDialect.ToString()), SqlGeneratorFactory.CreateMigrationGenerator(sqlDialect, _sqlExecuterTestAdapter.GetContext(sqlDialect)));

            databaseMigrator.DeleteTable(first);
        }
    }
}