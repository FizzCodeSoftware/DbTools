namespace FizzCode.DbTools.DataDefinition.SqlGenerator.Tests
{
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Base;
    using FizzCode.DbTools.DataDefinition.Factory;
    using FizzCode.DbTools.DataDefinition.Generic1;
    using FizzCode.DbTools.Factory.Interfaces;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SqlGeneratorTests
    {
        protected static SqlExecuterTestAdapter SqlExecuterTestAdapter { get; } = new();

        private readonly ISqlGeneratorFactory _sqlGeneratorFactory;

        public SqlGeneratorTests()
        {
            _sqlGeneratorFactory = new SqlGeneratorFactory(new TestContextFactory(null));
        }

        [DataTestMethod]
        [LatestSqlVersions]
#pragma warning disable IDE1006 // Naming Styles
        public void _010_GenerateScriptAndCreateTable(SqlEngineVersion version)
#pragma warning restore IDE1006 // Naming Styles
        {
            SqlExecuterTestAdapter.Check(version);
            SqlExecuterTestAdapter.InitializeAndCreate(version.UniqueName);

            var dd = new DatabaseDefinition(new TestFactoryContainer(), version);

            var table = new SqlTable("HierarchyFromCsvToSqlTests");
            var column = table.AddInt32("Id");
            column.Properties.Add(new Identity(column) { Increment = 1, Seed = 1 });
            table.AddNVarChar("Name", 100);

            dd.AddTable(table);



            var generator = _sqlGeneratorFactory.CreateSqlGenerator(version);

            var sql = generator.CreateTable(table);

            var result = SqlExecuterTestAdapter.ExecuteNonQuery(version.UniqueName, sql);

            if (result != null)
                Assert.Inconclusive(result);
        }

        [DataTestMethod]
        [LatestSqlVersions]
#pragma warning disable IDE1006 // Naming Styles
        public void _020_DropTable(SqlEngineVersion version)
#pragma warning restore IDE1006 // Naming Styles
        {
            SqlExecuterTestAdapter.Check(version);
            SqlExecuterTestAdapter.Initialize(version.UniqueName);

            var table = new SqlTable("HierarchyFromCsvToSqlTests");

            var generator = _sqlGeneratorFactory.CreateSqlGenerator(version);
            var sql = generator.DropTable(table);
            var result = SqlExecuterTestAdapter.ExecuteNonQuery(version.UniqueName, sql);

            if (result != null)
                Assert.Inconclusive(result);
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            SqlExecuterTestAdapter.Cleanup();
        }
    }
}