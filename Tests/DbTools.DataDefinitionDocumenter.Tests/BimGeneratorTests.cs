namespace FizzCode.DbTools.DataDefinitionDocumenter.Tests
{
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Generic1;
    using FizzCode.DbTools.DataDefinition.Tests;
    using FizzCode.DbTools.Tabular;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public static class DataDefinitionDocumenterTestsHelper
    {
        public static DocumenterContext CreateTestContext(ITableCustomizer customizer = null)
        {
            var context = new DocumenterContext
            {
                Settings = TestHelper.GetDefaultTestSettings(SqlVersions.MsSql2016),
                DocumenterSettings = new DocumenterSettings(),
                Customizer = customizer ?? new EmptyTableCustomizer(),
                Logger = TestHelper.CreateLogger()
            };

            return context;
        }
    }

    [TestClass]
    public class BimGeneratorTests
    {
        [TestMethod]
        [LatestSqlVersions]
        public void GeneratorTestDatabaseFks(SqlVersion version)
        {
            var db = new TestDatabaseFks();

            var generator = new BimGenerator(DataDefinitionDocumenterTestsHelper.CreateTestContext(new DocumenterTests.TableCustomizer()), version, "TestDatabaseFks");
            generator.Generate(db);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void GeneratorForeignKeyComposite(SqlVersion version)
        {
            var db = new ForeignKeyComposite();

            var generator = new BimGenerator(DataDefinitionDocumenterTestsHelper.CreateTestContext(), version, "TestDatabaseFks");
            generator.Generate(db);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void GeneratorForeignKeyComposite1(SqlVersion version)
        {
            var db = new ForeignKeyComposite();

            var generator = new BimGenerator(DataDefinitionDocumenterTestsHelper.CreateTestContext(), version, "ForeignKeyComposite");
            generator.Generate(db);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void GeneratorForeignKeyComposite2(SqlVersion version)
        {
            var db = new ForeignKeyCompositeSetForeignKeyTo();
            var generator = new BimGenerator(DataDefinitionDocumenterTestsHelper.CreateTestContext(new DocumenterTests.TableCustomizer()), version, "ForeignKeyCompositeSetForeignKeyTo");
            generator.Generate(db);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void GeneratorTabularRelation(SqlVersion version)
        {
            var db = new TabularRelation();

            var generator = new BimGenerator(DataDefinitionDocumenterTestsHelper.CreateTestContext(), version, "TabularRelation");
            generator.Generate(db);
        }
    }

    public class TabularRelation : DatabaseDeclaration
    {
        public SqlTable KeyTable { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddNVarChar("Value", 100);
        });

        public SqlTable ReferringTableA { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddInt32("KeyTableId1").SetForeignKeyTo("KeyTable").AddTabularRelation("dbo", "KeyTable", "Id", "Relation1");
            table.AddInt32("KeyTableId2").SetForeignKeyTo("KeyTable").AddTabularRelation("dbo", "KeyTable", "Id", "Relation2");
        });

        public SqlTable ReferringTableB { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddInt32("KeyTableId1").SetForeignKeyTo("KeyTable").AddTabularRelation("dbo", "KeyTable", "Id", "Relation1");
            table.AddInt32("KeyTableId2").SetForeignKeyTo("KeyTable").AddTabularRelation("dbo", "KeyTable", "Id", "Relation2");
        });
    }
}