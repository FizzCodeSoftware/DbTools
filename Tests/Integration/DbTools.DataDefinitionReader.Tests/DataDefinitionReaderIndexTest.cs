namespace FizzCode.DbTools.DataDefinitionReader.Tests
{
    using System.Linq;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.SqlExecuter;
    using FizzCode.DbTools.DataDefinition.Tests;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FizzCode.DbTools.DataDefinition.Factory;

    [TestClass]
    public class DataDefinitionReaderIndexTest : DataDefinitionReaderTests
    {
        [DataTestMethod]
        [LatestSqlVersions]
        public void CreateTables(SqlEngineVersion version)
        {
            var dd = new TestDatabaseIndex();
            Init(version, dd);
            var creator = new DatabaseCreator(dd, SqlExecuterTestAdapter.GetExecuter(version.UniqueName));
            creator.ReCreateDatabase(true);
        }

        [DataTestMethod]
        [LatestSqlVersions]
        public void ReadTables(SqlEngineVersion version)
        {
            TestHelper.CheckFeature(version, "ReadDdl");

            Init(version, null);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(
                SqlExecuterTestAdapter.ConnectionStrings[version.UniqueName],
                SqlExecuterTestAdapter.GetContext(version), null);
            var dd = ddlReader.GetDatabaseDefinition();

            var _ = dd.GetTable("Company").Properties.OfType<Index>().First();
        }
    }
}