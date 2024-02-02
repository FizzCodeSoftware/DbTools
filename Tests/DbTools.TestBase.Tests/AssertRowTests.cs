namespace FizzCode.DbTools.TestBase.Tests;

using FizzCode.DbTools.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class AssertRowTests
{
    [TestMethod]
    public void AssertRowTestEqual()
    {
        var expected = new Row() { { "Id", 1 }, { "Name", "CompanyName" } };
        var actual = new Row() { { "Id", 1 }, { "Name", "CompanyName" } };

        AssertRow.AreEqual(expected, actual, null);
    }

    private const string AssertRowTestExpectedMessage = @"AssertRow.AreEqual failed. First difference is on row number 1:
Id: 1, Id: 2


Expected followed by actual:
Id: 1
Name: CompanyName
Id: 2
Name: CompanyName
";
    [TestMethod]
    [ExpectedExceptionMessageStartsWith(typeof(AssertFailedException), AssertRowTestExpectedMessage)]
    public void AssertRowTestNotEqual()
    {
        var expected = new Row() { { "Id", 1 }, { "Name", "CompanyName" } };
        var actual = new Row() { { "Id", 2 }, { "Name", "CompanyName" } };

        AssertRow.AreEqual(expected, actual, null);
    }

    private const string AssertRowSetTestNotEqualExpectedMessage = @"AssertRowSet.AreEqual failed. First difference is on row number 2:
Name: SecondCompanyName, Name: SeYondCompanyName


Expected followed by actual:
Id: 2
Name: SecondCompanyName
Id: 2
Name: SeYondCompanyName
";

    [TestMethod]
    [ExpectedExceptionMessageStartsWithAttribute(typeof(AssertFailedException), AssertRowSetTestNotEqualExpectedMessage)]
    public void AssertRowSetTestNotEqual()
    {
        var expected = new RowSet() {
            new Row() { { "Id", 1 }, { "Name", "FirstCompanyName" } },
            new Row() { { "Id", 2 }, { "Name", "SecondCompanyName" } }
        };

        var actual = new RowSet() {
            new Row() { { "Id", 1 }, { "Name", "FirstCompanyName" } },
            new Row() { { "Id", 2 }, { "Name", "SeYondCompanyName" } }
        };

        AssertRowSet.AreEqual(expected, actual, null);
    }
}
