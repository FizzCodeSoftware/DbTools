namespace FizzCode.DbTools.DataDefinitionDocumenter.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FizzCode.DbTools.DataDefinition.Tests;

    [TestClass]
    public class DocumenterTests
    {
        [TestMethod]
        public void DocumentTest()
        {
            var db = new TestDatabaseFks();
            var documenter = new Documenter("TestDatabaseFks");
            documenter.Document(db);
        }
    }
}