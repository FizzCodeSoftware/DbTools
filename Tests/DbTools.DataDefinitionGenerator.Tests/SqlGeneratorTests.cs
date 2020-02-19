namespace FizzCode.DbTools.DataDefinition.SqlGenerator.Tests
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
        public void _010_GenerateScriptAndCreateTable(SqlEngineVersion version)
#pragma warning restore IDE1006 // Naming Styles
        {
            _sqlExecuterTestAdapter.Check(version);
            _sqlExecuterTestAdapter.InitializeAndCreate(version.UniqueName);

            var dd = new DatabaseDefinition(null, new[] { MsSqlVersion.MsSql2016.GetTypeMapper(), OracleVersion.Oracle12c.GetTypeMapper(), SqLiteVersion.SqLite3.GetTypeMapper() });

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

            var result = _sqlExecuterTestAdapter.ExecuteNonQuery(version.UniqueName, sql);

            if (result != null)
                Assert.Inconclusive(result);
        }

        [DataTestMethod]
        [LatestSqlVersions]
#pragma warning disable IDE1006 // Naming Styles
        public void _020_DropTable(SqlEngineVersion version)
#pragma warning restore IDE1006 // Naming Styles
        {
            _sqlExecuterTestAdapter.Check(version);
            _sqlExecuterTestAdapter.Initialize(version.UniqueName);

            var table = new SqlTable("HierarchyFromCsvToSqlTests");

            var generator = SqlGeneratorFactory.CreateGenerator(version, _sqlExecuterTestAdapter.GetContext(version));
            var sql = generator.DropTable(table);
            var result = _sqlExecuterTestAdapter.ExecuteNonQuery(version.UniqueName, sql);

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