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
            GeneratorTestDatabaseFks(MsSqlVersion.MsSql2016);
            GeneratorTestDatabaseFks(GenericVersion.Generic1);
            GeneratorTestDatabaseFks(OracleVersion.Oracle12c);
            GeneratorTestDatabaseFks(SqLiteVersion.SqLite3);
        }

        public void GeneratorTestDatabaseFks(SqlEngineVersion version)
        {
            var db = new TestDatabaseFks();

            var documenterContext = DataDefinitionDocumenterTestsHelper.CreateTestContext(new DocumenterTests.TableCustomizer());
            var writer = CSharpWriterFactory.GetCSharpWriter(version, documenterContext);
            var generator = new CSharpGenerator(documenterContext, writer, version, "TestDatabaseFks", "FizzCode.DbTools.DataDefinitionDocumenter.Tests");
            generator.GenerateMultiFile(db);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void GeneratorForeignKeyComposite(SqlEngineVersion version)
        {
            var db = new ForeignKeyComposite();

            var documenterContext = DataDefinitionDocumenterTestsHelper.CreateTestContext();
            var writer = CSharpWriterFactory.GetCSharpWriter(version, documenterContext);
            var generator = new CSharpGenerator(documenterContext, writer, version, "TestDatabaseFks", "FizzCode.DbTools.DataDefinitionDocumenter.Tests");
            generator.GenerateMultiFile(db);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void GeneratorForeignKeyComposite1(SqlEngineVersion version)
        {
            var db = new ForeignKeyComposite();

            var documenterContext = DataDefinitionDocumenterTestsHelper.CreateTestContext();
            var writer = CSharpWriterFactory.GetCSharpWriter(version, documenterContext);
            var generator = new CSharpGenerator(documenterContext, writer, version, "ForeignKeyComposite", "FizzCode.DbTools.DataDefinitionDocumenter.Tests");
            generator.GenerateMultiFile(db);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void GeneratorForeignKeyComposite2(SqlEngineVersion version)
        {
            var db = new ForeignKeyCompositeSetForeignKeyTo();
            var documenterContext = DataDefinitionDocumenterTestsHelper.CreateTestContext(new DocumenterTests.TableCustomizer());
            var writer = CSharpWriterFactory.GetCSharpWriter(version, documenterContext);
            var generator = new CSharpGenerator(documenterContext, writer, version, "ForeignKeyCompositeSetForeignKeyTo", "FizzCode.DbTools.DataDefinitionDocumenter.Tests");
            generator.GenerateMultiFile(db);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void GeneratorIndex(SqlEngineVersion version)
        {
            var db = new TestDatabaseIndex();
            var documenterContext = DataDefinitionDocumenterTestsHelper.CreateTestContext(new DocumenterTests.TableCustomizer());
            var writer = CSharpWriterFactory.GetCSharpWriter(version, documenterContext);
            var generator = new CSharpGenerator(documenterContext, writer, version, "TestDatabaseIndex", "FizzCode.DbTools.DataDefinitionDocumenter.Tests");
            generator.GenerateMultiFile(db);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void GeneratorUniqueIndex(SqlEngineVersion version)
        {
            var db = new TestDatabaseUniqueIndex();
            var documenterContext = DataDefinitionDocumenterTestsHelper.CreateTestContext(new DocumenterTests.TableCustomizer());
            var writer = CSharpWriterFactory.GetCSharpWriter(version, documenterContext);
            var generator = new CSharpGenerator(documenterContext, writer, version, "TestDatabaseUniqueIndex", "FizzCode.DbTools.DataDefinitionDocumenter.Tests");
            generator.GenerateMultiFile(db);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void GeneratorUniqueConstraint(SqlEngineVersion version)
        {
            var db = new TestDatabaseUniqueConstraint();
            var documenterContext = DataDefinitionDocumenterTestsHelper.CreateTestContext(new DocumenterTests.TableCustomizer());
            var writer = CSharpWriterFactory.GetCSharpWriter(version, documenterContext);
            var generator = new CSharpGenerator(documenterContext, writer, version, "TestDatabaseUniqueConstraint", "FizzCode.DbTools.DataDefinitionDocumenter.Tests");
            generator.GenerateMultiFile(db);
        }

        public class TestDatabaseUniqueConstraint : TestDatabaseDeclaration
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