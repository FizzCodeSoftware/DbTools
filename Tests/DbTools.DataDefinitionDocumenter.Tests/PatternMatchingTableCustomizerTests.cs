namespace FizzCode.DbTools.DataDefinitionDocumenter.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FizzCode.DbTools.DataDefinitionDocumenter;
    using System.Configuration;
    using System.IO;
    using FizzCode.DbTools.DataDefinition.Tests;
    using FizzCode.DbTools.TestBase;
    using FizzCode.DbTools.Common;

    [TestClass]
    public class PatternMatchingTableCustomizerTests
    {
        [TestMethod]
        public void GetPatternMatching_StartsWith_Test()
        {
            var pm = new PatternMatchingTableCustomizer();
            pm.AddPattern("A*", null, true, null, null);

            Assert.IsTrue(pm.ShouldSkip("A"));
            Assert.IsTrue(pm.ShouldSkip("Aaa"));
            Assert.IsTrue(pm.ShouldSkip("Abb"));
            Assert.IsFalse(pm.ShouldSkip("B"));
            Assert.IsFalse(pm.ShouldSkip("Baa"));
            Assert.IsFalse(pm.ShouldSkip("Bbb"));
        }

        [TestMethod]
        public void GetPatternMatching_Contains_Test()
        {
            var pm = new PatternMatchingTableCustomizer();
            pm.AddPattern("*apple*", null, true, null, null);

            Assert.IsTrue(pm.ShouldSkip("xapplex"));
            Assert.IsTrue(pm.ShouldSkip("apple"));
            Assert.IsTrue(pm.ShouldSkip("applex"));

            Assert.IsTrue(pm.ShouldSkip("The quick brown fox jumps over the lazy apple dog"));

            Assert.IsFalse(pm.ShouldSkip("appl"));
            Assert.IsFalse(pm.ShouldSkip("pple"));
            Assert.IsFalse(pm.ShouldSkip("xxx"));
        }

        [TestMethod]
        public void GetPatternMatching_Dot_Test()
        {
            var pm = new PatternMatchingTableCustomizer();
            pm.AddPattern("ap?le", null, true, null, null);

            Assert.IsTrue(pm.ShouldSkip("apple"));
            Assert.IsTrue(pm.ShouldSkip("apxle"));

            Assert.IsFalse(pm.ShouldSkip("The quick brown fox jumps over the lazy apXle dog"));

            Assert.IsTrue(pm.ShouldSkip("apXle The quick brown fox jumps over the lazy dog"));

            Assert.IsFalse(pm.ShouldSkip("appl"));
            Assert.IsFalse(pm.ShouldSkip("pple"));
            Assert.IsFalse(pm.ShouldSkip("xxx"));
        }

        [TestMethod]
        public void PatternMatchingTableCustomizerFromCsvTest()
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
            var documenter = new Documenter(TestHelper.GetDefaultTestSettings(SqlDialect.MsSql), "TestDatabaseFks", patternMatching);
            documenter.Document(db);
        }
    }
}