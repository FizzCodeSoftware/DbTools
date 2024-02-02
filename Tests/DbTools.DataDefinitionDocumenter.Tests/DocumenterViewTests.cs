namespace FizzCode.DbTools.DataDefinitionDocumenter.Tests;

using FizzCode.DbTools.DataDefinition.Tests;
using FizzCode.DbTools.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class DocumenterViewTests : DocumenterTestsBase
{
    [TestMethod]
    [LatestSqlVersions]
    public void DocumentViewTest(SqlEngineVersion version)
    {
        Document(new TestDatabaseSimpleWithView(), version);
    }
}
