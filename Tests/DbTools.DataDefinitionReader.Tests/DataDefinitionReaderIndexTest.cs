namespace FizzCode.DbTools.DataDefinitionReader.Tests
{
    using System.Linq;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.SqlExecuter;
    using FizzCode.DbTools.DataDefinition.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DataDefinitionReaderIndexTest : DataDefinitionReaderTests
    {
        [DataTestMethod]
        //[SqlVersions(typeof(MsSql2016))]
        //public void CreateTables(SqlVersion version)
        public void CreateTables()
        {
            var version = MsSqlVersion.MsSql2016;

            var dd = new TestDatabaseIndex();
            Init(version, dd);
            var creator = new DatabaseCreator(dd, _sqlExecuterTestAdapter.GetExecuter(version.UniqueName));
            creator.ReCreateDatabase(true);
        }

        [DataTestMethod]
        // [SqlVersions(typeof(MsSql2016))]
        // public void ReadTables(SqlVersion version)
        public void ReadTables()
        {
            var version = MsSqlVersion.MsSql2016;

            Init(version, null);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(
                _sqlExecuterTestAdapter.ConnectionStrings[version.UniqueName],
                _sqlExecuterTestAdapter.GetContext(version), null);
            var dd = ddlReader.GetDatabaseDefinition();

            var _ = dd.GetTable("Company").Properties.OfType<Index>().First();
        }
    }
}