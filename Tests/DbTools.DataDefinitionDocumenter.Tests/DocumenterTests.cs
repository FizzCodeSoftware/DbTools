namespace FizzCode.DbTools.DataDefinitionDocumenter.Tests
{
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DocumenterTests
    {
        [TestMethod]
        public void DocumentTest()
        {
            var db = new TestDatabaseFks();
            var documenter = new Documenter(DataDefinitionDocumenterTestsHelper.CreateTestContext(), "TestDatabaseFks");

            documenter.Document(db);
        }

        [TestMethod]
        public void TableCustomizerTest()
        {
            var db = new TestDatabaseFks();
            var documenter = new Documenter(DataDefinitionDocumenterTestsHelper.CreateTestContext(new TableCustomizer()), "TestDatabaseFks");
            documenter.Document(db);
        }

        [TestMethod]
        public void DocumentTestForeignKeyComposite()
        {
            var db = new ForeignKeyCompositeTestsDb();
            var documenter = new Documenter(DataDefinitionDocumenterTestsHelper.CreateTestContext(), "ForeignKeyCompositeTestsDb");
            documenter.Document(db);
        }

        public class TableCustomizer : ITableCustomizer
        {
            public string BackGroundColor(SchemaAndTableName tableName)
            {
                if (tableName.SchemaAndName == "Child")
                    return "#00FFFF";

                return null;
            }

            public string Category(SchemaAndTableName tableName)
            {
                if (tableName.SchemaAndName == "Child")
                    return "CategoryTest";

                return null;
            }

            public bool ShouldSkip(SchemaAndTableName tableName)
            {
                return tableName.SchemaAndName == "ChildChild";
            }
        }
    }
}