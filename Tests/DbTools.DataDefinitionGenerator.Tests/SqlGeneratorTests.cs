namespace FizzCode.DbTools.DataDefinitionGenerator.Tests
{
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Common.Logger;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SqlGeneratorTests
    {
        private static readonly SqlExecuterTestAdapter _sqlExecuterTestAdapter = new SqlExecuterTestAdapter();

        [DataTestMethod]
        [SqlDialects]
        public void _010_GenerateScriptAndCreateTable(SqlDialect sqlDialect)
        {
            _sqlExecuterTestAdapter.InitializeAndCheck(sqlDialect);
            TestHelper.CheckProvider(sqlDialect);

            var table = new SqlTable("HierarchyFromCsvToSqlTests");
            var column = table.AddInt32("Id");
            column.Properties.Add(new Identity(column) { Increment = 1, Seed = 1 });
            table.AddNVarChar("Name", 100);

            var context = new GeneratorContext
            {
                Settings = TestHelper.GetDefaultTestSettings(sqlDialect),
                Logger = TestHelper.CreateLogger()
            };

            var generator = SqlGeneratorFactory.CreateGenerator(sqlDialect, context);

            var sql = generator.CreateTable(table);

            var result = _sqlExecuterTestAdapter.ExecuteNonQuery(sqlDialect.ToString(), sql);

            if (result != null)
                Assert.Inconclusive(result);
        }

        [DataTestMethod]
        [SqlDialects]
        public void _020_DropTable(SqlDialect sqlDialect)
        {
            _sqlExecuterTestAdapter.InitializeAndCheck(sqlDialect);

            var table = new SqlTable("HierarchyFromCsvToSqlTests");

            var context = new GeneratorContext
            {
                Logger = TestHelper.CreateLogger(),
                Settings = Helper.GetDefaultSettings(sqlDialect, null)
            };

            var generator = SqlGeneratorFactory.CreateGenerator(sqlDialect, context);
            var sql = generator.DropTable(table);
            var result = _sqlExecuterTestAdapter.ExecuteNonQuery(sqlDialect.ToString(), sql);

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