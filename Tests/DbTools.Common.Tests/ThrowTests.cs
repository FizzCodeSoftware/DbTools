using FizzCode.DbTools.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FizzCode.DbTools.Common.Tests;
[TestClass]
public class ThrowTests
{

    [TestMethod]
    [ExpectedExceptionMessageStartsWith(typeof(InvalidOperationException), "row.GetAs<string>(\"ThisIsNull\") cannot be null.")]
    public void ThrowIfNull()
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var row = new Row
        {
            ["ThisIsNull"] = null
        };
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        
        var result = Throw.IfNull(row.GetAs<string>("ThisIsNull"));
    }

    [TestMethod]
    [ExpectedExceptionMessageStartsWith(typeof(InvalidOperationException), "row.GetAs<string>(\"ThisIsNull\") cannot be null.")]
    public void ThrowIfDbNull()
    {
        var row = new Row
        {
            ["ThisIsNull"] = DBNull.Value
        };

        var result = Throw.IfNull(row.GetAs<string>("ThisIsNull"));
    }

    [TestMethod]
    [ExpectedExceptionMessageStartsWith(typeof(InvalidOperationException), "nullString cannot be null. Additional message for nullString.")]
    public void ThrowInvalidOperationExceptionIfNullWithMessage()
    {
        string? nullString = null;
        Throw.InvalidOperationExceptionIfNull(nullString, message: "Additional message for nullString.");
    }
}