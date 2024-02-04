namespace FizzCode.DbTools.DataDefinitionDocumenter.Tests
{
    using FizzCode.DbTools.DataDefinition.Base;
    using FizzCode.DbTools.DataDefinition.Tests;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DocumenterTests : DocumenterTestsBase
    {
        [TestMethod]
        [LatestSqlVersions]
        public void DocumentTest(SqlEngineVersion version)
        {
            Document(new TestDatabaseFks(), version);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void TableCustomizerTest(SqlEngineVersion version)
        {
            var db = new TestDatabaseFks();
            db.SetVersions(version);
            var documenter = new Documenter(DocumenterTestsHelper.CreateTestDocumenterContext(version, new TableCustomizer()), version, "TestDatabaseFksWithTableCustomizer", "TestDatabaseFksWithTableCustomizer" + "_" + version + ".xlsx");
            documenter.Document(db);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void DocumentTestForeignKeyComposite(SqlEngineVersion version)
        {
            Document(new ForeignKeyComposite(), version);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void DocumentTestIndexMultiColumn(SqlEngineVersion version)
        {
            Document(new TestDatabaseIndexMultiColumn(), version);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void DocumentTestIndexMultiColumnAndInclude(SqlEngineVersion version)
        {
            Document(new TestDatabaseIndexMultiColumnAndInclude(), version);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void DocumentTestUniqueConstraint(SqlEngineVersion version)
        {
            Document(new TestDatabaseUniqueConstraint(), version);
        }

        internal class TableCustomizer : ITableCustomizer
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