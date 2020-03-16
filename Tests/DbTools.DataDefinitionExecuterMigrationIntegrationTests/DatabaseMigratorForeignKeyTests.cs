namespace FizzCode.DbTools.DataDefinition.SqlExecuterMigrationIntegration.Tests
{
    using System.Linq;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Generic1;
    using FizzCode.DbTools.DataDefinition.Migration;
    using FizzCode.DbTools.DataDefinition.SqlExecuter;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DatabaseMigratorForeignKeyTests : DataDefinitionExecuterMigrationIntegrationTests
    {
        [TestMethod]
        [LatestSqlVersions]
        public void AddFkTest(SqlEngineVersion version)
        {
            var dd = new TestDatabaseFk();
            dd.GetTable("Foreign").Properties.Remove(
                dd.GetTable("Foreign").Properties.OfType<ForeignKey>().First()
                );

            dd.SetVersions(version.GetTypeMapper());
            Init(version, dd);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(
                SqlExecuterTestAdapter.ConnectionStrings[version.UniqueName]
                , SqlExecuterTestAdapter.GetContext(version), dd.GetSchemaNames().ToList());
            var ddInDatabase = ddlReader.GetDatabaseDefinition();

            var ddWithFK = new TestDatabaseFk();
            ddWithFK.SetVersions(version.GetTypeMapper());

            var comparer = new Comparer(SqlExecuterTestAdapter.GetContext(version));
            var changes = comparer.Compare(ddInDatabase, ddWithFK);

            var first = changes[0] as ForeignKeyNew;

            var databaseMigrator = new DatabaseMigrator(SqlExecuterTestAdapter.GetExecuter(version.UniqueName), SqlGeneratorFactory.CreateMigrationGenerator(version, SqlExecuterTestAdapter.GetContext(version)));

            // TODO change FK
            // databaseMigrator.
        }

        [TestMethod]
        [LatestSqlVersions]
        public void RemoveFkTest(SqlEngineVersion version)
        {
            var dd = new TestDatabaseFk();
            dd.SetVersions(version.GetTypeMapper());
            Init(version, dd);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(
                SqlExecuterTestAdapter.ConnectionStrings[version.UniqueName]
                , SqlExecuterTestAdapter.GetContext(version), dd.GetSchemaNames().ToList());
            var ddInDatabase = ddlReader.GetDatabaseDefinition();

            var ddFKRemoved = new TestDatabaseFk();
            ddFKRemoved.GetTable("Foreign").Properties.Remove(
                ddFKRemoved.GetTable("Foreign").Properties.OfType<ForeignKey>().First()
                );
            ddFKRemoved.SetVersions(version.GetTypeMapper());

            var comparer = new Comparer(SqlExecuterTestAdapter.GetContext(version));
            var changes = comparer.Compare(ddInDatabase, ddFKRemoved);

            var first = changes[0] as ForeignKeyDelete;

            var databaseMigrator = new DatabaseMigrator(SqlExecuterTestAdapter.GetExecuter(version.UniqueName), SqlGeneratorFactory.CreateMigrationGenerator(version, SqlExecuterTestAdapter.GetContext(version)));

            // TODO change FK
            // databaseMigrator.
        }

        [TestMethod]
        [LatestSqlVersions]
        public void ChangeFkTest(SqlEngineVersion version)
        {
            var dd = new TestDatabaseFkChange();
            dd.SetVersions(version.GetTypeMapper());
            Init(version, dd);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(
                SqlExecuterTestAdapter.ConnectionStrings[version.UniqueName]
                , SqlExecuterTestAdapter.GetContext(version), dd.GetSchemaNames().ToList());
            var ddInDatabase = ddlReader.GetDatabaseDefinition();

            var ddFkChanged = new TestDatabaseFkChange();
            ddFkChanged.GetTable("Foreign").Properties.Remove(
                ddFkChanged.GetTable("Foreign").Properties.OfType<ForeignKey>().First()
                );
            ddFkChanged.GetTable("Foreign").Columns["PrimaryId"].SetForeignKeyTo("Primary2", "FkChange");
            ddFkChanged.SetVersions(version.GetTypeMapper());

            var comparer = new Comparer(SqlExecuterTestAdapter.GetContext(version));
            var changes = comparer.Compare(ddInDatabase, ddFkChanged);

            var first = changes[0] as ForeignKeyChange;

            var databaseMigrator = new DatabaseMigrator(SqlExecuterTestAdapter.GetExecuter(version.UniqueName), SqlGeneratorFactory.CreateMigrationGenerator(version, SqlExecuterTestAdapter.GetContext(version)));

            // TODO change FK
            // databaseMigrator.
        }

        [TestMethod]
        public void FkCheckNoCheckTest()
        {
            var version = MsSqlVersion.MsSql2016;

            var dd = new TestDatabaseFk();
            dd.SetVersions(version.GetTypeMapper());
            Init(version, dd);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(
                SqlExecuterTestAdapter.ConnectionStrings[version.UniqueName]
                , SqlExecuterTestAdapter.GetContext(version), dd.GetSchemaNames().ToList());
            var ddInDatabase = ddlReader.GetDatabaseDefinition();

            var fk = dd.GetTable("Foreign").Properties.OfType<ForeignKey>().First();

            Assert.AreEqual("true", fk.SqlEngineVersionSpecificProperties[version, "Nocheck"]);

            fk.SqlEngineVersionSpecificProperties[version, "Nocheck"] = "false";

            var comparer = new Comparer(SqlExecuterTestAdapter.GetContext(version));
            var changes = comparer.Compare(ddInDatabase, dd);

            var first = changes[0] as ForeignKeyChange;

            var databaseMigrator = new DatabaseMigrator(SqlExecuterTestAdapter.GetExecuter(version.UniqueName), SqlGeneratorFactory.CreateMigrationGenerator(version, SqlExecuterTestAdapter.GetContext(version)));

            // TODO change FK
            // databaseMigrator.
        }
    }

    public class TestDatabaseFk : TestDatabaseDeclaration
    {
        public SqlTable Primary { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK();
        });

        public SqlTable Foreign { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddInt32("PrimaryId").SetForeignKeyTo(nameof(Primary), new SqlEngineVersionSpecificProperty(MsSqlVersion.MsSql2016, "Nocheck", "true"));
        });
    }

    public class TestDatabaseFkChange : TestDatabaseDeclaration
    {
        public SqlTable Primary1 { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK();
        });

        public SqlTable Primary2 { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK();
        });

        public SqlTable Foreign { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddInt32("PrimaryId").SetForeignKeyTo(nameof(Primary1), "FkChange");
        });
    }
}