namespace FizzCode.DbTools.DataDefinitionDocumenter.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FizzCode.DbTools.DataDefinition.Tests;
    using System.IO;
    using System.Configuration;

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

        [TestMethod]
        public void PatternMatchingTableCustomizerTest()
        {
            var path = ConfigurationManager.AppSettings["WorkingDirectory"];
            using (var file =
            new StreamWriter(path + "TestDatabaseFks.DbTools.Patterns.csv"))
            {
                file.WriteLine("Pattern;PatternExcept;ShouldSkipIfMatch;CategoryIfMatch;BackGroundColorIfMatch");
                file.WriteLine("Parent;;0;Parent;606060");
                file.WriteLine("Child;;1");
                file.WriteLine("*ildC*;;0;TestCategory");
            }

            var db = new TestDatabaseFks();
            var patternMatching = new PatternMatchingTableCustomizerFromCsv("TestDatabaseFks");
            var documenter = new Documenter("TestDatabaseFks", patternMatching);
            documenter.Document(db);
        }
    }
}