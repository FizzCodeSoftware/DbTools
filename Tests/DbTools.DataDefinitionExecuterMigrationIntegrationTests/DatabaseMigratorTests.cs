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
            var dd = new TestDatabaseSimple();
            dd.SetVersions(version);
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
            dd.SetVersions(version, null);
            AddTable(dd);
            Init(version, dd);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(
                _sqlExecuterTestAdapter.ConnectionStrings[version.ToString()]
                , _sqlExecuterTestAdapter.GetContext(version), dd.GetSchemaNames().ToList());
            var ddInDatabase = ddlReader.GetDatabaseDefinition();

            var comparer = new Comparer(_sqlExecuterTestAdapter.GetContext(version));

            var ddOrigin = new TestDatabaseSimple();
            ddOrigin.SetVersions(version, null);

            var changes = comparer.Compare(ddInDatabase, ddOrigin);

            var first = changes[0] as TableDelete;
            Assert.AreEqual((SchemaAndTableName)"NewTableToMigrate", first.SchemaAndTableName);

            var databaseMigrator = new DatabaseMigrator(_sqlExecuterTestAdapter.GetExecuter(version.ToString()), SqlGeneratorFactory.CreateMigrationGenerator(version, _sqlExecuterTestAdapter.GetContext(version)));

            databaseMigrator.DeleteTable(first);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void RemoveColumnTest(SqlVersion version)
        {
            var dd = new TestDatabaseSimple();
            dd.SetVersions(version);
            Init(version, dd);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(
                _sqlExecuterTestAdapter.ConnectionStrings[version.ToString()]
                , _sqlExecuterTestAdapter.GetContext(version), dd.GetSchemaNames().ToList());
            var ddInDatabase = ddlReader.GetDatabaseDefinition();

            dd.GetTable("Company").Columns.Remove("Name");

            var comparer = new Comparer(_sqlExecuterTestAdapter.GetContext(version));
            var changes = comparer.Compare(ddInDatabase, dd);

            var first = changes[0] as ColumnDelete;
            Assert.AreEqual("Name", first.SqlColumn.Name);

            var databaseMigrator = new DatabaseMigrator(_sqlExecuterTestAdapter.GetExecuter(version.ToString()), SqlGeneratorFactory.CreateMigrationGenerator(version, _sqlExecuterTestAdapter.GetContext(version)));

            databaseMigrator.DeleteColumns(first);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void AddColumnTest(SqlVersion version)
        {
            var dd = new TestDatabaseSimple();
            dd.SetVersions(version);
            Init(version, dd);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(
                _sqlExecuterTestAdapter.ConnectionStrings[version.ToString()]
                , _sqlExecuterTestAdapter.GetContext(version), dd.GetSchemaNames().ToList());
            var ddInDatabase = ddlReader.GetDatabaseDefinition();

            dd.GetTable("Company").AddVarChar("Name2", 100);

            var comparer = new Comparer(_sqlExecuterTestAdapter.GetContext(version));
            var changes = comparer.Compare(ddInDatabase, dd);

            var first = changes[0] as ColumnNew;
            Assert.AreEqual("Name2", first.SqlColumn.Name);

            var databaseMigrator = new DatabaseMigrator(_sqlExecuterTestAdapter.GetExecuter(version.ToString()), SqlGeneratorFactory.CreateMigrationGenerator(version, _sqlExecuterTestAdapter.GetContext(version)));

            databaseMigrator.CreateColumns(first);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void Add2ColumnTest(SqlVersion version)
        {
            var dd = new TestDatabaseSimple();
            dd.SetVersions(version);
            Init(version, dd);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(
                _sqlExecuterTestAdapter.ConnectionStrings[version.ToString()]
                , _sqlExecuterTestAdapter.GetContext(version), dd.GetSchemaNames().ToList());
            var ddInDatabase = ddlReader.GetDatabaseDefinition();

            dd.GetTable("Company").AddVarChar("Name2", 100);
            dd.GetTable("Company").AddVarChar("Name3", 100, true);

            var comparer = new Comparer(_sqlExecuterTestAdapter.GetContext(version));
            var changes = comparer.Compare(ddInDatabase, dd);

            var first = changes[0] as ColumnNew;
            Assert.AreEqual("Name2", first.SqlColumn.Name);
            var second = changes[1] as ColumnNew;
            Assert.AreEqual("Name3", second.SqlColumn.Name);
            Assert.AreEqual(true, second.SqlColumn.Type.IsNullable);

            var databaseMigrator = new DatabaseMigrator(_sqlExecuterTestAdapter.GetExecuter(version.ToString()), SqlGeneratorFactory.CreateMigrationGenerator(version, _sqlExecuterTestAdapter.GetContext(version)));

            databaseMigrator.CreateColumns(first, second);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void ChangeColumnLengthTest(SqlVersion version)
        {
            var dd = new TestDatabaseSimple();
            dd.SetVersions(version);
            Init(version, dd);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(
                _sqlExecuterTestAdapter.ConnectionStrings[version.ToString()]
                , _sqlExecuterTestAdapter.GetContext(version), dd.GetSchemaNames().ToList());
            var ddInDatabase = ddlReader.GetDatabaseDefinition();

            dd.GetTable("Company")["Name"].Type.Length += 1;

            var comparer = new Comparer(_sqlExecuterTestAdapter.GetContext(version));
            var changes = comparer.Compare(ddInDatabase, dd);

            var first = changes[0] as ColumnChange;
            Assert.AreEqual(100, ddInDatabase.GetTable("Company")["Name"].Type.Length);
            Assert.AreEqual(100, first.SqlColumn.Type.Length);
            Assert.AreEqual(101, first.NewNameAndType.Type.Length);

            /*var databaseMigrator = new DatabaseMigrator(_sqlExecuterTestAdapter.GetExecuter(version.ToString()), SqlGeneratorFactory.CreateMigrationGenerator(version, _sqlExecuterTestAdapter.GetContext(version)));

            databaseMigrator.DeleteTable(first);*/
        }

        [TestMethod]
        [LatestSqlVersions]
        public void Remove2ColumnsTest(SqlVersion version)
        {
            var dd = new TestDatabaseSimple();
            dd.SetVersions(version, null);
            dd.GetTable("Company").AddNVarChar("Name2", 100);
            Init(version, dd);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(
                _sqlExecuterTestAdapter.ConnectionStrings[version.ToString()]
                , _sqlExecuterTestAdapter.GetContext(version), dd.GetSchemaNames().ToList());
            var ddInDatabase = ddlReader.GetDatabaseDefinition();

            dd.GetTable("Company").Columns.Remove("Name");
            dd.GetTable("Company").Columns.Remove("Name2");

            var comparer = new Comparer(_sqlExecuterTestAdapter.GetContext(version));
            var changes = comparer.Compare(ddInDatabase, dd);

            var first = changes[0] as ColumnDelete;
            Assert.AreEqual("Name", first.SqlColumn.Name);
            var second = changes[1] as ColumnDelete;
            Assert.AreEqual("Name2", second.SqlColumn.Name);

            var databaseMigrator = new DatabaseMigrator(_sqlExecuterTestAdapter.GetExecuter(version.ToString()), SqlGeneratorFactory.CreateMigrationGenerator(version, _sqlExecuterTestAdapter.GetContext(version)));

            databaseMigrator.DeleteColumns(first, second);
        }
    }
}