namespace FizzCode.DbTools.DataDefinitionDocumenter.Tests
{
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Generic1;
    using FizzCode.DbTools.DataDefinition.Tests;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CsGeneratorTests
    {
        [TestMethod]
        public void GeneratorTestDatabaseFks()
        {
            GeneratorTestDatabaseFks(SqlVersions.MsSql2016);
            GeneratorTestDatabaseFks(SqlVersions.Generic1);
            GeneratorTestDatabaseFks(SqlVersions.Oracle12c);
            GeneratorTestDatabaseFks(SqlVersions.SqLite3);
        }

        public void GeneratorTestDatabaseFks(SqlVersion version)
        {
            var db = new TestDatabaseFks();

            var generator = new CsGenerator(DataDefinitionDocumenterTestsHelper.CreateTestContext(new DocumenterTests.TableCustomizer()), version, "TestDatabaseFks", "FizzCode.DbTools.DataDefinitionDocumenter.Tests");
            generator.GenerateMultiFile(db);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void GeneratorForeignKeyComposite(SqlVersion version)
        {
            var db = new ForeignKeyComposite();

            var generator = new CsGenerator(DataDefinitionDocumenterTestsHelper.CreateTestContext(), version, "TestDatabaseFks", "FizzCode.DbTools.DataDefinitionDocumenter.Tests");
            generator.GenerateMultiFile(db);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void GeneratorForeignKeyComposite1(SqlVersion version)
        {
            var db = new ForeignKeyComposite();

            var generator = new CsGenerator(DataDefinitionDocumenterTestsHelper.CreateTestContext(), version, "ForeignKeyComposite", "FizzCode.DbTools.DataDefinitionDocumenter.Tests");
            generator.GenerateMultiFile(db);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void GeneratorForeignKeyComposite2(SqlVersion version)
        {
            var db = new ForeignKeyCompositeSetForeignKeyTo();
            var generator = new CsGenerator(DataDefinitionDocumenterTestsHelper.CreateTestContext(new DocumenterTests.TableCustomizer()), version, "ForeignKeyCompositeSetForeignKeyTo", "FizzCode.DbTools.DataDefinitionDocumenter.Tests");
            generator.GenerateMultiFile(db);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void GeneratorIndex(SqlVersion version)
        {
            var db = new TestDatabaseIndex();
            var generator = new CsGenerator(DataDefinitionDocumenterTestsHelper.CreateTestContext(new DocumenterTests.TableCustomizer()), version, "TestDatabaseIndex", "FizzCode.DbTools.DataDefinitionDocumenter.Tests");
            generator.GenerateMultiFile(db);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void GeneratorUniqueIndex(SqlVersion version)
        {
            var db = new TestDatabaseUniqueIndex();
            var generator = new CsGenerator(DataDefinitionDocumenterTestsHelper.CreateTestContext(new DocumenterTests.TableCustomizer()), version, "TestDatabaseUniqueIndex", "FizzCode.DbTools.DataDefinitionDocumenter.Tests");
            generator.GenerateMultiFile(db);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void GeneratorUniqueConstraint(SqlVersion version)
        {
            var db = new TestDatabaseUniqueConstraint();
            var generator = new CsGenerator(DataDefinitionDocumenterTestsHelper.CreateTestContext(new DocumenterTests.TableCustomizer()), version, "TestDatabaseUniqueConstraint", "FizzCode.DbTools.DataDefinitionDocumenter.Tests");
            generator.GenerateMultiFile(db);
        }

        public class TestDatabaseUniqueConstraint : DatabaseDeclaration
        {
            public SqlTable Company { get; } = AddTable(table =>
            {
                table.AddInt32("Id").SetPK().SetIdentity();
                table.AddNVarChar("Name", 100);
                table.AddUniqueConstraint("Name");
            });
        }
    }
}