namespace FizzCode.DbTools.DataDefinitionReader.Tests
{
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition.Tests;
    using FizzCode.DbTools.DataDefinitionExecuter;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DataDefinitionReaderDatabaseCircular2FKTests
    {
        private static readonly SqlExecuterTestAdapter _sqlExecuterTestAdatper = new SqlExecuterTestAdapter();

        [DataTestMethod]
        [SqlDialects]
        public void CreateTables(SqlDialect sqlDialect)
        {
            _sqlExecuterTestAdatper.Initialize(sqlDialect.ToString());

            var creator = new DatabaseCreator(new TestDatabaseCircular2FK(), _sqlExecuterTestAdatper.GetExecuter(sqlDialect.ToString()));
            creator.ReCreateDatabase(true);
        }

        [DataTestMethod]
        [SqlDialects]
        public void ReadTables(SqlDialect sqlDialect)
        {
            // var dd = new TestDatabaseCircular2FK();
            if (sqlDialect == SqlDialect.SqLite)
                Assert.Inconclusive("Test is skipped, no known way to read DDL with SqLite in memory.");

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(sqlDialect, _sqlExecuterTestAdatper.GetExecuter(sqlDialect.ToString()));
            _ = ddlReader.GetDatabaseDefinition();
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            _sqlExecuterTestAdatper.Cleanup();
        }
    }
}