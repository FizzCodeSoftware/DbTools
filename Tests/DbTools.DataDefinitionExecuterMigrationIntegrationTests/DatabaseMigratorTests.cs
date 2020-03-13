namespace FizzCode.DbTools.DataDefinition.SqlExecuterMigrationIntegration.Tests
{
    using System.Linq;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Generic1;
    using FizzCode.DbTools.DataDefinition.Migration;
    using FizzCode.DbTools.DataDefinition.SqlExecuter;
    using FizzCode.DbTools.DataDefinition.Tests;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DatabaseMigratorTests : DataDefinitionExecuterMigrationIntegrationTests
    {
        [TestMethod]
        [LatestSqlVersions]
        public void AddTableTest(SqlEngineVersion version)
        {
            var dd = new TestDatabaseSimple();
            dd.SetVersions(version.GetTypeMapper());
            Init(version, dd);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(SqlExecuterTestAdapter.ConnectionStrings[version.UniqueName], SqlExecuterTestAdapter.GetContext(version), dd.GetSchemaNames().ToList());
            var ddInDatabase = ddlReader.GetDatabaseDefinition();

            AddTable(dd);

            var comparer = new Comparer(SqlExecuterTestAdapter.GetContext(version));
            var changes = comparer.Compare(ddInDatabase, dd);

            var first = changes[0] as TableNew;
            Assert.AreEqual((SchemaAndTableName)"NewTableToMigrate", first.SchemaAndTableName);

            var databaseMigrator = new DatabaseMigrator(SqlExecuterTestAdapter.GetExecuter(version.UniqueName), SqlGeneratorFactory.CreateMigrationGenerator(version, SqlExecuterTestAdapter.GetContext(version)));

            databaseMigrator.NewTable(first);
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
            var dd = new TestDatabaseSimple();
            dd.SetVersions(version.GetTypeMapper());
            AddTable(dd);
            Init(version, dd);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(
                SqlExecuterTestAdapter.ConnectionStrings[version.UniqueName]
                , SqlExecuterTestAdapter.GetContext(version), dd.GetSchemaNames().ToList());
            var ddInDatabase = ddlReader.GetDatabaseDefinition();

            var comparer = new Comparer(SqlExecuterTestAdapter.GetContext(version));

            var ddOrigin = new TestDatabaseSimple();
            ddOrigin.SetVersions(version.GetTypeMapper());

            var changes = comparer.Compare(ddInDatabase, ddOrigin);

            var first = changes[0] as TableDelete;
            Assert.AreEqual((SchemaAndTableName)"NewTableToMigrate", first.SchemaAndTableName);

            var databaseMigrator = new DatabaseMigrator(SqlExecuterTestAdapter.GetExecuter(version.UniqueName), SqlGeneratorFactory.CreateMigrationGenerator(version, SqlExecuterTestAdapter.GetContext(version)));

            databaseMigrator.DeleteTable(first);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void RemoveColumnTest(SqlEngineVersion version)
        {
            var dd = new TestDatabaseSimple();
            dd.SetVersions(version.GetTypeMapper());
            Init(version, dd);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(
                SqlExecuterTestAdapter.ConnectionStrings[version.UniqueName]
                , SqlExecuterTestAdapter.GetContext(version), dd.GetSchemaNames().ToList());
            var ddInDatabase = ddlReader.GetDatabaseDefinition();

            dd.GetTable("Company").Columns.Remove("Name");

            var comparer = new Comparer(SqlExecuterTestAdapter.GetContext(version));
            var changes = comparer.Compare(ddInDatabase, dd);

            var first = changes[0] as ColumnDelete;
            Assert.AreEqual("Name", first.SqlColumn.Name);

            var databaseMigrator = new DatabaseMigrator(SqlExecuterTestAdapter.GetExecuter(version.UniqueName), SqlGeneratorFactory.CreateMigrationGenerator(version, SqlExecuterTestAdapter.GetContext(version)));

            databaseMigrator.DeleteColumns(first);
        }

        // TODO implement Defaultvalue, generate change order (default before CulumNew) in Comparer
        /*
        [TestMethod]
        [LatestSqlVersions]
        public void AddColumnNotNullWithDefaultValueTest(SqlEngineVersion version)
        {
            var dd = new TestDatabaseSimple();
            dd.SetVersions(version.GetTypeMapper());
            Init(version, dd);

            _sqlExecuterTestAdapter.GetExecuter(version.UniqueName).ExecuteNonQuery("INSERT INTO Company (Name) VALUES ('AddColumnNotNullTestValue')");

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(
                _sqlExecuterTestAdapter.ConnectionStrings[version.UniqueName]
                , _sqlExecuterTestAdapter.GetContext(version), dd.GetSchemaNames().ToList());
            var ddInDatabase = ddlReader.GetDatabaseDefinition();

            dd.GetTable("Company").AddVarChar("Name2", 100, false).AddDefaultValue("'default'");

            var comparer = new Comparer(_sqlExecuterTestAdapter.GetContext(version));
            var changes = comparer.Compare(ddInDatabase, dd);

            var first = changes[0] as ColumnNew;
            Assert.AreEqual("Name2", first.SqlColumn.Name);

            var databaseMigrator = new DatabaseMigrator(_sqlExecuterTestAdapter.GetExecuter(version.UniqueName), SqlGeneratorFactory.CreateMigrationGenerator(version, _sqlExecuterTestAdapter.GetContext(version)));

            databaseMigrator.CreateColumns(first);
        }*/

        [TestMethod]
        [LatestSqlVersions]
        public void AddColumnTest(SqlEngineVersion version)
        {
            var dd = new TestDatabaseSimple();
            dd.SetVersions(version.GetTypeMapper());
            Init(version, dd);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(
                SqlExecuterTestAdapter.ConnectionStrings[version.UniqueName]
                , SqlExecuterTestAdapter.GetContext(version), dd.GetSchemaNames().ToList());
            var ddInDatabase = ddlReader.GetDatabaseDefinition();

            dd.GetTable("Company").AddVarChar("Name2", 100);

            var comparer = new Comparer(SqlExecuterTestAdapter.GetContext(version));
            var changes = comparer.Compare(ddInDatabase, dd);

            var first = changes[0] as ColumnNew;
            Assert.AreEqual("Name2", first.SqlColumn.Name);

            var databaseMigrator = new DatabaseMigrator(SqlExecuterTestAdapter.GetExecuter(version.UniqueName), SqlGeneratorFactory.CreateMigrationGenerator(version, SqlExecuterTestAdapter.GetContext(version)));

            databaseMigrator.CreateColumns(first);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void Add2ColumnTest(SqlEngineVersion version)
        {
            var dd = new TestDatabaseSimple();
            dd.SetVersions(version.GetTypeMapper());
            Init(version, dd);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(
                SqlExecuterTestAdapter.ConnectionStrings[version.UniqueName]
                , SqlExecuterTestAdapter.GetContext(version), dd.GetSchemaNames().ToList());
            var ddInDatabase = ddlReader.GetDatabaseDefinition();

            dd.GetTable("Company").AddVarChar("Name2", 100);
            dd.GetTable("Company").AddVarChar("Name3", 100, true);

            var comparer = new Comparer(SqlExecuterTestAdapter.GetContext(version));
            var changes = comparer.Compare(ddInDatabase, dd);

            var first = changes[0] as ColumnNew;
            Assert.AreEqual("Name2", first.SqlColumn.Name);
            var second = changes[1] as ColumnNew;
            Assert.AreEqual("Name3", second.SqlColumn.Name);
            Assert.AreEqual(true, second.SqlColumn.Type.IsNullable);

            var databaseMigrator = new DatabaseMigrator(SqlExecuterTestAdapter.GetExecuter(version.UniqueName), SqlGeneratorFactory.CreateMigrationGenerator(version, SqlExecuterTestAdapter.GetContext(version)));

            databaseMigrator.CreateColumns(first, second);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void ChangeColumnLengthTest(SqlEngineVersion version)
        {
            var dd = new TestDatabaseSimple();
            dd.SetVersions(version.GetTypeMapper());
            Init(version, dd);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(
                SqlExecuterTestAdapter.ConnectionStrings[version.UniqueName]
                , SqlExecuterTestAdapter.GetContext(version), dd.GetSchemaNames().ToList());
            var ddInDatabase = ddlReader.GetDatabaseDefinition();

            dd.GetTable("Company")["Name"].Type.Length += 1;

            var comparer = new Comparer(SqlExecuterTestAdapter.GetContext(version));
            var changes = comparer.Compare(ddInDatabase, dd);

            var first = changes[0] as ColumnChange;
            Assert.AreEqual(100, ddInDatabase.GetTable("Company")["Name"].Type.Length);
            Assert.AreEqual(100, first.SqlColumn.Type.Length);
            Assert.AreEqual(101, first.NewNameAndType.Type.Length);

            var databaseMigrator = new DatabaseMigrator(SqlExecuterTestAdapter.GetExecuter(version.UniqueName), SqlGeneratorFactory.CreateMigrationGenerator(version, SqlExecuterTestAdapter.GetContext(version)));

            databaseMigrator.ChangeColumns(first);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void ChangeColumnNullableTest(SqlEngineVersion version)
        {
            var dd = new TestDatabaseSimple();
            dd.SetVersions(version.GetTypeMapper());
            Init(version, dd);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(
                SqlExecuterTestAdapter.ConnectionStrings[version.UniqueName]
                , SqlExecuterTestAdapter.GetContext(version), dd.GetSchemaNames().ToList());
            var ddInDatabase = ddlReader.GetDatabaseDefinition();

            dd.GetTable("Company")["Name"].Type.IsNullable = !dd.GetTable("Company")["Name"].Type.IsNullable;

            var comparer = new Comparer(SqlExecuterTestAdapter.GetContext(version));
            var changes = comparer.Compare(ddInDatabase, dd);

            var first = changes[0] as ColumnChange;
            Assert.AreEqual(false, ddInDatabase.GetTable("Company")["Name"].Type.IsNullable);
            Assert.AreEqual(false, first.SqlColumn.Type.IsNullable);
            Assert.AreEqual(true, first.NewNameAndType.Type.IsNullable);

            var databaseMigrator = new DatabaseMigrator(SqlExecuterTestAdapter.GetExecuter(version.UniqueName), SqlGeneratorFactory.CreateMigrationGenerator(version, SqlExecuterTestAdapter.GetContext(version)));

            databaseMigrator.ChangeColumns(first);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void Change2ColumnLengthTest(SqlEngineVersion version)
        {
            var dd = new TestDatabaseSimple();
            dd.SetVersions(version.GetTypeMapper());
            dd.GetTable("Company").AddNVarChar("Name2", 100);
            Init(version, dd);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(
                SqlExecuterTestAdapter.ConnectionStrings[version.UniqueName]
                , SqlExecuterTestAdapter.GetContext(version), dd.GetSchemaNames().ToList());
            var ddInDatabase = ddlReader.GetDatabaseDefinition();

            dd.GetTable("Company")["Name"].Type.Length += 1;
            dd.GetTable("Company")["Name2"].Type.Length += 1;

            var comparer = new Comparer(SqlExecuterTestAdapter.GetContext(version));
            var changes = comparer.Compare(ddInDatabase, dd);

            Assert.AreEqual(100, ddInDatabase.GetTable("Company")["Name"].Type.Length);
            Assert.AreEqual(100, ddInDatabase.GetTable("Company")["Name2"].Type.Length);
            var first = changes[0] as ColumnChange;
            Assert.AreEqual(100, first.SqlColumn.Type.Length);
            Assert.AreEqual(101, first.NewNameAndType.Type.Length);
            var second = changes[1] as ColumnChange;
            Assert.AreEqual(100, second.SqlColumn.Type.Length);
            Assert.AreEqual(101, second.NewNameAndType.Type.Length);

            var databaseMigrator = new DatabaseMigrator(SqlExecuterTestAdapter.GetExecuter(version.UniqueName), SqlGeneratorFactory.CreateMigrationGenerator(version, SqlExecuterTestAdapter.GetContext(version)));

            databaseMigrator.ChangeColumns(first, second);
        }

        [TestMethod]
        [SqlVersions(nameof(MsSql2016), nameof(Oracle12c))]
        public void ChangeColumnDbTypeTest(SqlEngineVersion version)
        {
            var dd = new TestDatabaseSimple();
            dd.SetVersions(version.GetTypeMapper());
            Init(version, dd);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(
                SqlExecuterTestAdapter.ConnectionStrings[version.UniqueName]
                , SqlExecuterTestAdapter.GetContext(version), dd.GetSchemaNames().ToList());
            var ddInDatabase = ddlReader.GetDatabaseDefinition();

            if (version == MsSqlVersion.MsSql2016)
                dd.GetTable("Company")["Name"].Type.SqlTypeInfo = MsSqlType2016.NChar;
            else if (version == OracleVersion.Oracle12c)
                dd.GetTable("Company")["Name"].Type.SqlTypeInfo = OracleType12c.NChar;

            var comparer = new Comparer(SqlExecuterTestAdapter.GetContext(version));
            var changes = comparer.Compare(ddInDatabase, dd);

            var first = changes[0] as ColumnChange;

            if (version == MsSqlVersion.MsSql2016)
            {
                Assert.IsTrue(ddInDatabase.GetTable("Company")["Name"].Type.SqlTypeInfo is MsSql2016.SqlNVarChar);
                Assert.IsTrue(first.SqlColumn.Type.SqlTypeInfo is MsSql2016.SqlNVarChar);
                Assert.IsTrue(first.NewNameAndType.Type.SqlTypeInfo is MsSql2016.SqlNChar);
            }
            else if (version == OracleVersion.Oracle12c)
            {
                Assert.IsTrue(ddInDatabase.GetTable("Company")["Name"].Type.SqlTypeInfo is Oracle12c.SqlNVarChar2);
                Assert.IsTrue(first.SqlColumn.Type.SqlTypeInfo is Oracle12c.SqlNVarChar2);
                Assert.IsTrue(first.NewNameAndType.Type.SqlTypeInfo is Oracle12c.SqlNChar);
            }

            var databaseMigrator = new DatabaseMigrator(SqlExecuterTestAdapter.GetExecuter(version.UniqueName), SqlGeneratorFactory.CreateMigrationGenerator(version, SqlExecuterTestAdapter.GetContext(version)));

            databaseMigrator.ChangeColumns(first);
        }

        [TestMethod]
        //[SqlVersions(nameof(MsSql2016), nameof(Oracle12c))]
        [SqlVersions(nameof(Oracle12c))]
        public void ChangeColumnMultipleTest(SqlEngineVersion version)
        {
            var dd = new TestDatabaseSimple();
            // dd.DefaultSchema = _sqlExecuterTestAdapter.GetContext(version).Settings.SqlVersionSpecificSettings["OracleDatabaseName"];
            dd.SetVersions(version.GetTypeMapper());
            Init(version, dd);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(
                SqlExecuterTestAdapter.ConnectionStrings[version.UniqueName]
                , SqlExecuterTestAdapter.GetContext(version), dd.GetSchemaNames().ToList());
            var ddInDatabase = ddlReader.GetDatabaseDefinition();

            if (version == MsSqlVersion.MsSql2016)
                dd.GetTable("Company")["Name"].Type.SqlTypeInfo = MsSqlType2016.NChar;
            else if (version == OracleVersion.Oracle12c)
                dd.GetTable("Company")["Name"].Type.SqlTypeInfo = OracleType12c.NChar;

            dd.GetTable("Company")["Name"].Type.Length += 1;
            dd.GetTable("Company")["Name"].Type.IsNullable = !dd.GetTable("Company")["Name"].Type.IsNullable;

            var comparer = new Comparer(SqlExecuterTestAdapter.GetContext(version));
            var changes = comparer.Compare(ddInDatabase, dd);

            var first = changes[0] as ColumnChange;

            if (version == MsSqlVersion.MsSql2016)
            {
                Assert.IsTrue(ddInDatabase.GetTable("Company")["Name"].Type.SqlTypeInfo is MsSql2016.SqlNVarChar);
                Assert.IsTrue(first.SqlColumn.Type.SqlTypeInfo is MsSql2016.SqlNVarChar);
                Assert.IsTrue(first.NewNameAndType.Type.SqlTypeInfo is MsSql2016.SqlNChar);
            }
            else if (version == OracleVersion.Oracle12c)
            {
                Assert.IsTrue(ddInDatabase.GetTable("Company")["Name"].Type.SqlTypeInfo is Oracle12c.SqlNVarChar2);
                Assert.IsTrue(first.SqlColumn.Type.SqlTypeInfo is Oracle12c.SqlNVarChar2);
                Assert.IsTrue(first.NewNameAndType.Type.SqlTypeInfo is Oracle12c.SqlNChar);
            }

            Assert.AreEqual(100, ddInDatabase.GetTable("Company")["Name"].Type.Length);
            Assert.AreEqual(100, first.SqlColumn.Type.Length);
            Assert.AreEqual(101, first.NewNameAndType.Type.Length);
            Assert.AreEqual(false, ddInDatabase.GetTable("Company")["Name"].Type.IsNullable);
            Assert.AreEqual(false, first.SqlColumn.Type.IsNullable);
            Assert.AreEqual(true, first.NewNameAndType.Type.IsNullable);

            var databaseMigrator = new DatabaseMigrator(SqlExecuterTestAdapter.GetExecuter(version.UniqueName), SqlGeneratorFactory.CreateMigrationGenerator(version, SqlExecuterTestAdapter.GetContext(version)));

            databaseMigrator.ChangeColumns(first);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void Remove2ColumnsTest(SqlEngineVersion version)
        {
            var dd = new TestDatabaseSimple();
            dd.SetVersions(version.GetTypeMapper());
            dd.GetTable("Company").AddNVarChar("Name2", 100);
            Init(version, dd);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(
                SqlExecuterTestAdapter.ConnectionStrings[version.UniqueName]
                , SqlExecuterTestAdapter.GetContext(version), dd.GetSchemaNames().ToList());
            var ddInDatabase = ddlReader.GetDatabaseDefinition();

            dd.GetTable("Company").Columns.Remove("Name");
            dd.GetTable("Company").Columns.Remove("Name2");

            var comparer = new Comparer(SqlExecuterTestAdapter.GetContext(version));
            var changes = comparer.Compare(ddInDatabase, dd);

            var first = changes[0] as ColumnDelete;
            Assert.AreEqual("Name", first.SqlColumn.Name);
            var second = changes[1] as ColumnDelete;
            Assert.AreEqual("Name2", second.SqlColumn.Name);

            var databaseMigrator = new DatabaseMigrator(SqlExecuterTestAdapter.GetExecuter(version.UniqueName), SqlGeneratorFactory.CreateMigrationGenerator(version, SqlExecuterTestAdapter.GetContext(version)));

            databaseMigrator.DeleteColumns(first, second);
        }
    }
}