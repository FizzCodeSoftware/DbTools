#pragma warning disable CA1034 // Nested types should not be visible

namespace FizzCode.DbTools.DataDefinitionReader.Tests
{
    using System.Linq;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.SqlExecuter;
    using FizzCode.DbTools.DataDefinition.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DataDefinitionReaderUniqueConstraintTest : DataDefinitionReaderTests
    {
        [TestMethod]
        // [SqlVersions(typeof(MsSql2016))]
        //public void CreateTables(SqlVersion version)
        public void CreateTables()
        {
            var version = MsSqlVersion.MsSql2016;
            var dd = new TestDatabaseUniqueConstraint();
            Init(version, dd);
            var creator = new DatabaseCreator(dd, SqlExecuterTestAdapter.GetExecuter(version.UniqueName));
            creator.ReCreateDatabase(true);
        }

        [TestMethod]
        //[SqlVersions(typeof(MsSql2016))]
        //public void ReadTables(SqlVersion version)
        public void ReadTables()
        {
            var version = MsSqlVersion.MsSql2016;

            Init(version, null);

            var dd = ReadDd(version, null);

            var _ = dd.GetTable("Company").Properties.OfType<UniqueConstraint>().First();
        }
    }
}