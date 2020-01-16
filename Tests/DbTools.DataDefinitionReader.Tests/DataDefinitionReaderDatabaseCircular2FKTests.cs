namespace FizzCode.DbTools.DataDefinitionReader.Tests
{
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition.Tests;
    using FizzCode.DbTools.DataDefinitionExecuter;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DataDefinitionReaderDatabaseCircular2FKTests : DataDefinitionReaderTests
    {
        [DataTestMethod]
        [LatestSqlVersions]
        public void CreateTables(SqlVersion version)
        {
            var dd = new TestDatabaseCircular2FK();
            Init(version, dd);
            var creator = new DatabaseCreator(dd, _sqlExecuterTestAdapter.GetExecuter(version.ToString()));
            creator.ReCreateDatabase(true);
        }

        [DataTestMethod]
        [LatestSqlVersions]
        public void ReadTables(SqlVersion version)
        {
            Init(version, null);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(
                _sqlExecuterTestAdapter.ConnectionStrings[version.ToString()],
                _sqlExecuterTestAdapter.GetContext(version));
            _ = ddlReader.GetDatabaseDefinition();
        }
    }
}