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

        [TestMethod]
        public void TableCustomizerTest()
        {
            var db = new TestDatabaseFks();
            var documenter = new Documenter("TestDatabaseFks", new TableCustomizer());
            documenter.Document(db);
        }

        public class TableCustomizer : ITableCustomizer
        {
            public string BackGroundColor(string tableName)
            {
                if (tableName == "Child")
                    return "#00FFFF";

                return null;
            }

            public string Category(string tableName)
            {
                if (tableName == "Child")
                    return "CategoryTest";

                return null;
            }

            public bool ShouldSkip(string tableName)
            {
                if (tableName == "ChildChild")
                    return true;

                return false;
            }
        }
    }
}