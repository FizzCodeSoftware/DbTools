namespace FizzCode.DbTools.DataDefinitionDocumenter.Tests
{
    using FizzCode.DbTools.DataDefinition.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CsGeneratorTests
    {
        [TestMethod]
        public void GeneratorTestDatabaseFks()
        {
            var db = new TestDatabaseFks();

            var generator = new CsGenerator(DataDefinitionDocumenterTestsHelper.CreateTestContext(new DocumenterTests.TableCustomizer()), "TestDatabaseFks", "FizzCode.DbTools.DataDefinitionDocumenter.Tests");
            generator.GenerateMultiFile(db);
        }

        [TestMethod]
        public void GeneratorForeignKeyComposite()
        {
            var db = new ForeignKeyComposite();

            var generator = new CsGenerator(DataDefinitionDocumenterTestsHelper.CreateTestContext(), "TestDatabaseFks", "FizzCode.DbTools.DataDefinitionDocumenter.Tests");
            generator.GenerateMultiFile(db);
        }

        [TestMethod]
        public void GeneratorForeignKeyComposite1()
        {
            var db = new ForeignKeyComposite();

            var generator = new CsGenerator(DataDefinitionDocumenterTestsHelper.CreateTestContext(), "ForeignKeyComposite", "FizzCode.DbTools.DataDefinitionDocumenter.Tests");
            generator.GenerateMultiFile(db);
        }

        [TestMethod]
        public void GeneratorForeignKeyComposite2()
        {
            var db = new ForeignKeyCompositeSetForeignKeyTo();
            var generator = new CsGenerator(DataDefinitionDocumenterTestsHelper.CreateTestContext(new DocumenterTests.TableCustomizer()), "ForeignKeyCompositeSetForeignKeyTo", "FizzCode.DbTools.DataDefinitionDocumenter.Tests");
            generator.GenerateMultiFile(db);
        }
    }
}