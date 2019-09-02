namespace FizzCode.DbTools.DataDefinitionReader.Tests
{
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition.Tests;
    using FizzCode.DbTools.DataDefinitionExecuter;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public abstract class DataDefinitionReaderTests
    {
        protected static readonly SqlExecuterTestAdapter _sqlExecuterTestAdapter = new SqlExecuterTestAdapter();

        [AssemblyCleanup]
        public static void Cleanup()
        {
            _sqlExecuterTestAdapter.Cleanup();
        }
    }

    [TestClass]
    public class DataDefinitionReaderDatabaseCircular2FKTests : DataDefinitionReaderTests
    {
        //private static readonly SqlExecuterTestAdapter _sqlExecuterTestAdapter = new SqlExecuterTestAdapter();

        [DataTestMethod]
        [SqlDialects]
        public void CreateTables(SqlDialect sqlDialect)
        {
            _sqlExecuterTestAdapter.Initialize(sqlDialect.ToString());
            var creator = new DatabaseCreator(new TestDatabaseCircular2FK(), _sqlExecuterTestAdapter.GetExecuter(sqlDialect.ToString()));
            creator.ReCreateDatabase(true);
        }

        [DataTestMethod]
        [SqlDialects]
        public void ReadTables(SqlDialect sqlDialect)
        {
            TestHelper.CheckFeature(sqlDialect, "ReadDdl");

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(sqlDialect, _sqlExecuterTestAdapter.GetExecuter(sqlDialect.ToString()));
            _ = ddlReader.GetDatabaseDefinition();
        }
    }
}