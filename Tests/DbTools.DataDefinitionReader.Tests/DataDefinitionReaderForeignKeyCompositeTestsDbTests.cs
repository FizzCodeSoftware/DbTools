namespace FizzCode.DbTools.DataDefinitionReader.Tests
{
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition.Tests;
    using FizzCode.DbTools.DataDefinitionExecuter;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DataDefinitionReaderForeignKeyCompositeTestsDbTests : DataDefinitionReaderTests
    {
        [DataTestMethod]
        [SqlDialects]
        public void CreateTables(SqlDialect sqlDialect)
        {
            var dd = new TestDataBaseForeignKeyComposite();
            _sqlExecuterTestAdapter.InitializeAndCheck(sqlDialect, dd);

            var creator = new DatabaseCreator(dd, _sqlExecuterTestAdapter.GetExecuter(sqlDialect.ToString()));
            creator.ReCreateDatabase(true);
        }

        [DataTestMethod]
        [SqlDialects]
        public void ReadTables(SqlDialect sqlDialect)
        {
            TestHelper.CheckFeature(sqlDialect, "ReadDdl");
            TestHelper.CheckProvider(sqlDialect);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(sqlDialect, _sqlExecuterTestAdapter.GetExecuter(sqlDialect.ToString()));
            _ = ddlReader.GetDatabaseDefinition();
        }
    }
}