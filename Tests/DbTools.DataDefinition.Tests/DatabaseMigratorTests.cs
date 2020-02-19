namespace FizzCode.DbTools.DataDefinition.Tests
{
    using System.Linq;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Generic1;
    using FizzCode.DbTools.DataDefinition.Migration;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DatabaseMigratorTests
    {
        public Context GetContext(SqlEngineVersion version)
        {
            var context = new Context
            {
                Settings = TestHelper.GetDefaultTestSettings(version),
                Logger = TestHelper.CreateLogger()
            };

            return context;
        }

        [TestMethod]
        [LatestSqlVersions]
        public void AddTableTest(SqlEngineVersion version)
        {
            var ddOriginal = new TestDatabaseSimple();
            ddOriginal.SetVersions(version.GetTypeMapper());

            var dd = new TestDatabaseSimple();
            dd.SetVersions(version.GetTypeMapper());
            AddTable(dd);

            var comparer = new Comparer(GetContext(version));
            var changes = comparer.Compare(ddOriginal, dd);

            var first = changes[0] as TableNew;
            Assert.AreEqual((SchemaAndTableName)"NewTableToMigrate", first.SchemaAndTableName);
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
        public void RemoveTableTest(SqlEngineVersion version)
        {
            var ddOriginal = new TestDatabaseSimple();
            ddOriginal.SetVersions(version.GetTypeMapper());
            AddTable(ddOriginal);

            var dd = new TestDatabaseSimple();
            dd.SetVersions(version.GetTypeMapper());

            var comparer = new Comparer(GetContext(version));

            var ddOrigin = new TestDatabaseSimple();
            ddOrigin.SetVersions(version.GetTypeMapper());

            var changes = comparer.Compare(ddOriginal, ddOrigin);

            var first = changes[0] as TableDelete;
            Assert.AreEqual((SchemaAndTableName)"NewTableToMigrate", first.SchemaAndTableName);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void RemoveColumnTest(SqlEngineVersion version)
        {
            var originalDd = new TestDatabaseSimple();
            originalDd.SetVersions(version.GetTypeMapper());

            var dd = new TestDatabaseSimple();
            dd.SetVersions(version.GetTypeMapper());

            dd.GetTable("Company").Columns.Remove("Name");

            var comparer = new Comparer(GetContext(version));
            var changes = comparer.Compare(originalDd, dd);

            var first = changes[0] as ColumnDelete;
            Assert.AreEqual("Name", first.SqlColumn.Name);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void AddColumnTest(SqlEngineVersion version)
        {
            var ddOriginal = new TestDatabaseSimple();
            ddOriginal.SetVersions(version.GetTypeMapper());

            var dd = new TestDatabaseSimple();
            dd.SetVersions(version.GetTypeMapper());

            dd.GetTable("Company").AddVarChar("Name2", 100);

            var comparer = new Comparer(GetContext(version));
            var changes = comparer.Compare(ddOriginal, dd);

            var first = changes[0] as ColumnNew;
            Assert.AreEqual("Name2", first.SqlColumn.Name);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void Add2ColumnTest(SqlEngineVersion version)
        {
            var ddOriginal = new TestDatabaseSimple();
            ddOriginal.SetVersions(version.GetTypeMapper());

            var dd = new TestDatabaseSimple();
            dd.SetVersions(version.GetTypeMapper());

            dd.GetTable("Company").AddVarChar("Name2", 100);
            dd.GetTable("Company").AddVarChar("Name3", 100, true);

            var comparer = new Comparer(GetContext(version));
            var changes = comparer.Compare(ddOriginal, dd);

            var first = changes[0] as ColumnNew;
            Assert.AreEqual("Name2", first.SqlColumn.Name);
            var second = changes[1] as ColumnNew;
            Assert.AreEqual("Name3", second.SqlColumn.Name);
            Assert.AreEqual(true, second.SqlColumn.Type.IsNullable);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void ChangeColumnLengthTest(SqlEngineVersion version)
        {
            TestHelper.CheckFeature(version, "ColumnLength");

            var ddOriginal = new TestDatabaseSimple();
            ddOriginal.SetVersions(version.GetTypeMapper());

            var dd = new TestDatabaseSimple();
            dd.SetVersions(version.GetTypeMapper());

            dd.GetTable("Company")["Name"].Type.Length += 1;

            var comparer = new Comparer(GetContext(version));
            var changes = comparer.Compare(ddOriginal, dd);

            var first = changes[0] as ColumnChange;

            Assert.AreEqual(100, ddOriginal.GetTable("Company")["Name"].Type.Length);
            Assert.AreEqual(100, first.SqlColumn.Type.Length);
            Assert.AreEqual(101, first.NewNameAndType.Type.Length);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void Remove2ColumnsTest(SqlEngineVersion version)
        {
            var ddOriginal = new TestDatabaseSimple();
            ddOriginal.SetVersions(version.GetTypeMapper());
            ddOriginal.GetTable("Company").AddNVarChar("Name2", 100);

            var dd = new TestDatabaseSimple();
            dd.SetVersions(version.GetTypeMapper());

            dd.GetTable("Company").Columns.Remove("Name");
            dd.GetTable("Company").Columns.Remove("Name2");

            var comparer = new Comparer(GetContext(version));
            var changes = comparer.Compare(ddOriginal, dd);

            var first = changes[0] as ColumnDelete;
            Assert.AreEqual("Name", first.SqlColumn.Name);
            var second = changes[1] as ColumnDelete;
            Assert.AreEqual("Name2", second.SqlColumn.Name);
        }
    }
}