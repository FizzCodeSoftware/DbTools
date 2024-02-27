using FizzCode.DbTools.DataDefinition.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.TestBase;
using FizzCode.DbTools.DataDefinition;
using FizzCode.DbTools.DataDefinition.Base.Migration;

namespace FizzCode.DbTools.DataDefinitionReader.Tests;
[TestClass]
public class DataDefinitionReaderDefaultValueTest : DataDefinitionReaderTests
{
    [TestMethod]
    [LatestSqlVersions]
    public void ReadDefaultValue(SqlEngineVersion version)
    {
        var ddOriginal = new TestDatabaseSimple();
        ddOriginal.GetTable("Company")["Name"].AddDefaultValue("'apple'");
        var nameColumnOriginal = ddOriginal.GetTable("Company")["Name"];
        var dvOriginal = Assert.That.CheckAndReturnInstanceOfType<DefaultValue>(nameColumnOriginal.Properties[0]);

        Init(version, ddOriginal);

        var dd = ReadDd(version, null);
        var nameColumn = dd.GetTable("Company")["Name"];

        Assert.AreEqual(1, nameColumn.Properties.Count);
        var dv = Assert.That.CheckAndReturnInstanceOfType<DefaultValue>(nameColumn.Properties[0]);

        var comparer = new ComparerDefaultValue();
        Assert.IsTrue(comparer.CompareProperties(dvOriginal, dv));
    }
}