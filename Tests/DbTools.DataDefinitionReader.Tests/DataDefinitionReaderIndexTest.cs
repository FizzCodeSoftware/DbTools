namespace FizzCode.DbTools.DataDefinitionReader.Tests
{
    using System.Linq;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Tests;
    using FizzCode.DbTools.DataDefinitionExecuter;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DataDefinitionReaderIndexTest : DataDefinitionReaderTests
    {
        [DataTestMethod]
        [LatestSqlVersions]
        public void CreateTables(SqlVersion version)
        {
            var dd = new TestDatabaseIndex();
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
                _sqlExecuterTestAdapter.GetContext(version), null);
            var dd = ddlReader.GetDatabaseDefinition();

            var index = dd.GetTable("Company").Properties.OfType<Index>().First();
        }
    }
}