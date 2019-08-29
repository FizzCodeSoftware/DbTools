namespace FizzCode.DbTools.DataDefinitionGenerator.Tests
{
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SqlGeneratorTests
    {
        private static readonly SqlExecuterTestAdapter _sqlExecuterTestAdatper = new SqlExecuterTestAdapter();

        [DataTestMethod]
        [SqlDialects]
        public void _010_GenerateScriptAndCreateTable(SqlDialect sqlDialect)
        {
            _sqlExecuterTestAdatper.Initialize(sqlDialect.ToString());

            var table = new SqlTable("HierarchyFromCsvToSqlTests");
            var column = table.AddInt32("Id");
            column.Properties.Add(new Identity(column) { Increment = 1, Seed = 1 });
            table.AddNVarChar("Name", 100);

            var generator = SqlGeneratorFactory.CreateGenerator(sqlDialect, Common.Helper.GetDefaultTestSettings(sqlDialect));

            var sql = generator.CreateTable(table);

            var result = _sqlExecuterTestAdatper.ExecuteNonQuery(sqlDialect.ToString(), sql);

            if (result != null)
                Assert.Inconclusive(result);
        }

        [DataTestMethod]
        [SqlDialects]
        public void _020_DropTable(SqlDialect sqlDialect)
        {
            _sqlExecuterTestAdatper.Initialize(sqlDialect.ToString());

            var table = new SqlTable("HierarchyFromCsvToSqlTests");

            var generator = SqlGeneratorFactory.CreateGenerator(sqlDialect, Common.Helper.GetDefaultTestSettings(sqlDialect));
            var sql = generator.DropTable(table);
            var result = _sqlExecuterTestAdatper.ExecuteNonQuery(sqlDialect.ToString(), sql);

            if (result != null)
                Assert.Inconclusive(result);
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            _sqlExecuterTestAdatper.Cleanup();
        }
    }
}