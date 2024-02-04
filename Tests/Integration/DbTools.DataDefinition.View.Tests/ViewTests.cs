using FizzCode.DbTools.Common;
using FizzCode.DbTools.DataDefinition.Tests;
using FizzCode.DbTools.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FizzCode.DbTools.DataDefinition.View.Tests;
[TestClass]
public class ViewTests : ViewTestsBase
{
    [TestMethod]
    [LatestSqlVersions]
    public void ViewSimple(SqlEngineVersion version)
    {
        Init(version, new TestDatabaseSimpleWithView());

        // TODO TEST IDENTITY (Creation, Insert, Check inserted values)

        SqlExecuterTestAdapter.ExecuteWithPrepareNonQuery(version.UniqueName, "INSERT INTO [Company] ([Name]) VALUES ('FirstCompanyName')");
        SqlExecuterTestAdapter.ExecuteWithPrepareNonQuery(version.UniqueName, "INSERT INTO [Company] ([Name]) VALUES ('SecondCompanyName')");

        var result = SqlExecuterTestAdapter.ExecuteWithPrepareQuery(version.UniqueName, "SELECT * FROM [CompanyView]");

        Assert.AreEqual(2, result.Count);

        var expected = new RowSet() { 
            new Row() { { "Id", 1 }, { "Name", "FirstCompanyName" } },
            new Row() { { "Id", 2 }, { "Name", "SecondCompanyName" } },
            };

        AssertRowSet.AreEqual(expected, result, version);
    }
}
