namespace FizzCode.DbTools.DataDefinitionReader.Tests
{
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.SqlExecuter;
    using FizzCode.DbTools.DataDefinition.Tests;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DataDefinitionReaderDatabaseCircular2FKTests : DataDefinitionReaderTests
    {
        [DataTestMethod]
        [LatestSqlVersions]
        public void CreateTables(SqlEngineVersion version)
        {
            var dd = new TestDatabaseCircular2FK();
            Init(version, dd);
            var creator = new DatabaseCreator(dd, SqlExecuterTestAdapter.GetExecuter(version.UniqueName));
            creator.ReCreateDatabase(true);
        }

        [DataTestMethod]
        [LatestSqlVersions]
        public void ReadTables(SqlEngineVersion version)
        {
            Init(version, null);

            _ = ReadDd(version, null);
        }
    }
}