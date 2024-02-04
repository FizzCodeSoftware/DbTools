using FizzCode.DbTools.DataDefinition.Tests;
using FizzCode.DbTools.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FizzCode.DbTools.DataDefinitionReader.Tests;
[TestClass]
public class DataDefinitionReaderDatabaseCircular2FKTests : DataDefinitionReaderTests
{
    [DataTestMethod]
    [LatestSqlVersions]
    public void ReadTables(SqlEngineVersion version)
    {
        Init(version, new TestDatabaseCircular2FK());

        _ = ReadDd(version, null);
    }
}