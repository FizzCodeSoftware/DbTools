namespace FizzCode.DbTools.DataDefinitionExecuterMigrationIntegration.Tests
{
    using System.Linq;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Generic1;
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
        [LatestSqlVersions]
        public void AddTableTest(SqlVersion version)
        {
            // Create Test Db
            // Read up DD
            // Add a table
            // Detect changes (one new table)
            // Execute changes/migration

            var dd = new TestDatabaseSimple();

            Init(version, dd);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(_sqlExecuterTestAdapter.ConnectionStrings[version.ToString()], _sqlExecuterTestAdapter.GetContext(version), dd.GetSchemaNames().ToList());
            var ddInDatabase = ddlReader.GetDatabaseDefinition();

            AddTable(dd);

            var comparer = new Comparer(_sqlExecuterTestAdapter.GetContext(version));
            var changes = comparer.Compare(ddInDatabase, dd);

            var first = changes[0] as TableNew;
            Assert.AreEqual((SchemaAndTableName)"NewTableToMigrate", first.SchemaAndTableName);

            var databaseMigrator = new DatabaseMigrator(_sqlExecuterTestAdapter.GetExecuter(version.ToString()), SqlGeneratorFactory.CreateMigrationGenerator(version, _sqlExecuterTestAdapter.GetContext(version)));

            databaseMigrator.NewTable(first);
        }

        private static void Init(SqlVersion version, DatabaseDefinition dd)
        {
            _sqlExecuterTestAdapter.Check(version);
            _sqlExecuterTestAdapter.Initialize(version.ToString(), dd);
            TestHelper.CheckFeature(version, "ReadDdl");

            _sqlExecuterTestAdapter.GetContext(version).Settings.Options.ShouldUseDefaultSchema = true;

            var databaseCreator = new DatabaseCreator(dd, _sqlExecuterTestAdapter.GetExecuter(version.ToString()));

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
        [LatestSqlVersions]
        public void RemoveTableTest(SqlVersion version)
        {
            var dd = new TestDatabaseSimple();
            AddTable(dd);
            Init(version, dd);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(
                _sqlExecuterTestAdapter.ConnectionStrings[version.ToString()]
                , _sqlExecuterTestAdapter.GetContext(version), dd.GetSchemaNames().ToList());
            var ddInDatabase = ddlReader.GetDatabaseDefinition();

            var comparer = new Comparer(_sqlExecuterTestAdapter.GetContext(version));
            var changes = comparer.Compare(ddInDatabase, new TestDatabaseSimple());

            var first = changes[0] as TableDelete;
            Assert.AreEqual((SchemaAndTableName)"NewTableToMigrate", first.SchemaAndTableName);

            var databaseMigrator = new DatabaseMigrator(_sqlExecuterTestAdapter.GetExecuter(version.ToString()), SqlGeneratorFactory.CreateMigrationGenerator(version, _sqlExecuterTestAdapter.GetContext(version)));

            databaseMigrator.DeleteTable(first);
        }
    }
}