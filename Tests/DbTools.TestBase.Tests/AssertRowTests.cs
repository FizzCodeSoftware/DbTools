namespace FizzCode.DbTools.TestBase.Tests;

using FizzCode.DbTools.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class AssertRowTests
{
    [TestMethod]
    public void CompareTest()
    {
        var expexted = new Row() { { "Id", 1 }, { "Name", "CompanyName" } };
        var actual = new Row() { { "Id", 2 }, { "Name", "CompanyName" } };

        AssertRow.Compare(expexted, actual, null, out var message);

        var x = message;
    }
}

