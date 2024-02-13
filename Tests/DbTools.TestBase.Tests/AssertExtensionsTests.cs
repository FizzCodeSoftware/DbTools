using FizzCode.DbTools.DataDefinition.Base.Migration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FizzCode.DbTools.TestBase.Tests;
[TestClass]
public class AssertExtensionsTests
{
    [TestMethod]
    [ExpectedExceptionMessageStartsWith(typeof(AssertFailedException), "AssertExtensions.CheckAndReturnInstanceOfType failed, value is null (type is ColumnDelete).")]
    public void CheckAndReturnInstanceOfType_Null()
    {
        string? value = null;
        Assert.That.CheckAndReturnInstanceOfType<ColumnDelete>(value);
    }

    [TestMethod]
    [ExpectedExceptionMessageStartsWith(typeof(AssertFailedException), "AssertExtensions.CheckAndReturnInstanceOfType failed, ColumnDelete is not assignable from ColumnNew.")]
    public void CheckAndReturnInstanceOfType_NotInstance()
    {
        IMigration value = new ColumnNew() { SqlColumn = new DataDefinition.Base.SqlColumn()};
        Assert.That.CheckAndReturnInstanceOfType<ColumnDelete>(value);
    }

    [TestMethod]
    public void CheckAndReturnInstanceOfType()
    {
        IMigration value = new ColumnDelete() { SqlColumn = new DataDefinition.Base.SqlColumn() };
        Assert.That.CheckAndReturnInstanceOfType<ColumnDelete>(value);
    }
}
