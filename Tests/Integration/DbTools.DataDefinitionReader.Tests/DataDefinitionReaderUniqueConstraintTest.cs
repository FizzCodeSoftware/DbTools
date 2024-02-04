using System.Linq;
using FizzCode.DbTools.DataDefinition.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDefinitionReader.Tests;
[TestClass]
public class DataDefinitionReaderUniqueConstraintTest : DataDefinitionReaderTests
{
    [TestMethod]
    //[SqlVersions(typeof(MsSql2016))]
    //public void ReadTables(SqlVersion version)
    public void ReadTables()
    {
        var version = MsSqlVersion.MsSql2016;

        Init(version, new TestDatabaseUniqueConstraint());

        var dd = ReadDd(version, null);

        var _ = dd.GetTable("Company").Properties.OfType<UniqueConstraint>().First();
    }
}