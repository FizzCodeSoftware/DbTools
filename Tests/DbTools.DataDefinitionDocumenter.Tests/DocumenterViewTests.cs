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
        // Note that columns of views are not generated from DatabaseDefinition
        // (like 'new ViewFromQuery(new Query(Company))'.
        // Columns of views are read only from Database engines.
        Document(new TestDatabaseSimpleWithView(), version);
    }
}
