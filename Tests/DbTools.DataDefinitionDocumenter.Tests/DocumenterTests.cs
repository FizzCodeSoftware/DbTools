namespace FizzCode.DbTools.DataDefinitionDocumenter.Tests
{
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Tests;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DocumenterTests
    {
        [TestMethod]
        [LatestSqlVersions]
        public void DocumentTest(SqlVersion version)
        {
            var db = new TestDatabaseFks();
            db.SetVersions(version);
            var documenter = new Documenter(DataDefinitionDocumenterTestsHelper.CreateTestContext(), version, "TestDatabaseFks");

            documenter.Document(db);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void TableCustomizerTest(SqlVersion version)
        {
            var db = new TestDatabaseFks();
            db.SetVersions(version);
            var documenter = new Documenter(DataDefinitionDocumenterTestsHelper.CreateTestContext(new TableCustomizer()), version, "TestDatabaseFks");
            documenter.Document(db);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void DocumentTestForeignKeyComposite(SqlVersion version)
        {
            var db = new ForeignKeyComposite();
            db.SetVersions(version);
            var documenter = new Documenter(DataDefinitionDocumenterTestsHelper.CreateTestContext(), version, "ForeignKeyComposite");
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