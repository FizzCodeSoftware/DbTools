namespace FizzCode.DbTools.DataDefinitionGenerator.Tests
{
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Generic1;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SqlGeneratorTests
    {
        private static readonly SqlExecuterTestAdapter _sqlExecuterTestAdapter = new SqlExecuterTestAdapter();

        [DataTestMethod]
        [LatestSqlVersions]
#pragma warning disable IDE1006 // Naming Styles
        public void _010_GenerateScriptAndCreateTable(SqlVersion version)
#pragma warning restore IDE1006 // Naming Styles
        {
            _sqlExecuterTestAdapter.Check(version);
            _sqlExecuterTestAdapter.InitializeAndCreate(version.ToString());

            var dd = new DatabaseDefinition();

            var table = new SqlTable("HierarchyFromCsvToSqlTests");
            var column = table.AddInt32("Id");
            column.Properties.Add(new Identity(column) { Increment = 1, Seed = 1 });
            table.AddNVarChar("Name", 100);

            dd.AddTable(table);

            var context = new Context
            {
                Settings = TestHelper.GetDefaultTestSettings(version),
                Logger = TestHelper.CreateLogger()
            };

            var generator = SqlGeneratorFactory.CreateGenerator(version, context);

            var sql = generator.CreateTable(table);

            var result = _sqlExecuterTestAdapter.ExecuteNonQuery(version.ToString(), sql);

            if (result != null)
                Assert.Inconclusive(result);
        }

        [DataTestMethod]
        [LatestSqlVersions]
#pragma warning disable IDE1006 // Naming Styles
        public void _020_DropTable(SqlVersion version)
#pragma warning restore IDE1006 // Naming Styles
        {
            _sqlExecuterTestAdapter.Check(version);
            _sqlExecuterTestAdapter.Initialize(version.ToString());

            var table = new SqlTable("HierarchyFromCsvToSqlTests");

            var context = new Context
            {
                Logger = TestHelper.CreateLogger(),
                Settings = Helper.GetDefaultSettings(version, null)
            };

            var generator = SqlGeneratorFactory.CreateGenerator(version, context);
            var sql = generator.DropTable(table);
            var result = _sqlExecuterTestAdapter.ExecuteNonQuery(version.ToString(), sql);

            if (result != null)
                Assert.Inconclusive(result);
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            _sqlExecuterTestAdapter.Cleanup();
        }
    }
}