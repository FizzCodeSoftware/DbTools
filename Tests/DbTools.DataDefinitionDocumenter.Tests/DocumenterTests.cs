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
        public void DocumentTest(SqlEngineVersion version)
        {
            var db = new TestDatabaseFks();
            db.SetVersions(version.GetTypeMapper());
            var documenter = new Documenter(DataDefinitionDocumenterTestsHelper.CreateTestContext(version), version, "TestDatabaseFks");

            documenter.Document(db);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void TableCustomizerTest(SqlEngineVersion version)
        {
            var db = new TestDatabaseFks();
            db.SetVersions(version.GetTypeMapper());
            var documenter = new Documenter(DataDefinitionDocumenterTestsHelper.CreateTestContext(version, new TableCustomizer()), version, "TestDatabaseFks");
            documenter.Document(db);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void DocumentTestForeignKeyComposite(SqlEngineVersion version)
        {
            var db = new ForeignKeyComposite();
            db.SetVersions(version.GetTypeMapper());
            var documenter = new Documenter(DataDefinitionDocumenterTestsHelper.CreateTestContext(version), version, "ForeignKeyComposite");
            documenter.Document(db);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void DocumentTestIndexMultiColumn(SqlEngineVersion version)
        {
            var db = new TestDatabaseIndexMultiColumn();
            db.SetVersions(version.GetTypeMapper());
            var documenter = new Documenter(DataDefinitionDocumenterTestsHelper.CreateTestContext(version), version, "TestDatabaseIndexMultiColumn");
            documenter.Document(db);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void DocumentTestIndexMultiColumnAndInclude(SqlEngineVersion version)
        {
            var db = new TestDatabaseIndexMultiColumnAndInclude();
            db.SetVersions(version.GetTypeMapper());
            var documenter = new Documenter(DataDefinitionDocumenterTestsHelper.CreateTestContext(version), version, "TestDatabaseIndexMultiColumnAndInclude");
            documenter.Document(db);
        }

        // 
        [TestMethod]
        [LatestSqlVersions]
        public void DocumentTestUniqueConstraint(SqlEngineVersion version)
        {
            var db = new TestDatabaseUniqueConstraint();
            db.SetVersions(version.GetTypeMapper());
            var documenter = new Documenter(DataDefinitionDocumenterTestsHelper.CreateTestContext(version), version, "TestDatabaseUniqueConstraint");
            documenter.Document(db);
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