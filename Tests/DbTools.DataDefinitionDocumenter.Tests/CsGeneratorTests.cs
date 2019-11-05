namespace FizzCode.DbTools.DataDefinitionDocumenter.Tests
{
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition.Tests;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CsGeneratorTests
    {
        [TestMethod]
        public void GeneratorTestDatabaseFks()
        {
            var db = new TestDatabaseFks();

            var generator = new CsGenerator(new DocumenterSettings(), TestHelper.GetDefaultTestSettings(SqlDialect.MsSql), "TestDatabaseFks", "FizzCode.DbTools.DataDefinitionDocumenter.Tests", new DocumenterTests.TableCustomizer());
            generator.GenerateMultiFile(db);
        }

        [TestMethod]
        public void GeneratorForeignKeyCompositeTestsDb()
        {
            var db = new ForeignKeyCompositeTestsDb();

            var generator = new CsGenerator(new DocumenterSettings(), TestHelper.GetDefaultTestSettings(SqlDialect.MsSql), "TestDatabaseFks", "FizzCode.DbTools.DataDefinitionDocumenter.Tests");
            generator.GenerateMultiFile(db);
        }

        [TestMethod]
        public void GeneratorForeignKeyCompositeTestsDb1()
        {
            var db = new ForeignKeyCompositeTestsDb();

            var generator = new CsGenerator(new DocumenterSettings(), TestHelper.GetDefaultTestSettings(SqlDialect.MsSql), "ForeignKeyCompositeTestsDb", "FizzCode.DbTools.DataDefinitionDocumenter.Tests");
            generator.GenerateMultiFile(db);
        }

        [TestMethod]
        public void GeneratorForeignKeyCompositeTestsDb2()
        {
            var db = new ForeignKeyCompositeSetForeignKeyToTestDb();
            var generator = new CsGenerator(new DocumenterSettings(), TestHelper.GetDefaultTestSettings(SqlDialect.MsSql), "ForeignKeyCompositeSetForeignKeyToTestDb", "FizzCode.DbTools.DataDefinitionDocumenter.Tests", new DocumenterTests.TableCustomizer());
            generator.GenerateMultiFile(db);
        }
    }
}