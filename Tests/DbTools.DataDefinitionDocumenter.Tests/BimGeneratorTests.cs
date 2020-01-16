namespace FizzCode.DbTools.DataDefinitionDocumenter.Tests
{
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Common.Logger;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
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
                Settings = TestHelper.GetDefaultTestSettings(new MsSql2016()),
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
        public void GeneratorTestDatabaseFks()
        {
            var db = new TestDatabaseFks();

            var generator = new BimGenerator(DataDefinitionDocumenterTestsHelper.CreateTestContext(new DocumenterTests.TableCustomizer()), "TestDatabaseFks");
            generator.Generate(db);
        }

        [TestMethod]
        public void GeneratorForeignKeyCompositeTestsDb()
        {
            var db = new ForeignKeyCompositeTestsDb();

            var generator = new BimGenerator(DataDefinitionDocumenterTestsHelper.CreateTestContext(), "TestDatabaseFks");
            generator.Generate(db);
        }

        [TestMethod]
        public void GeneratorForeignKeyCompositeTestsDb1()
        {
            var db = new ForeignKeyCompositeTestsDb();

            var generator = new BimGenerator(DataDefinitionDocumenterTestsHelper.CreateTestContext(), "ForeignKeyCompositeTestsDb");
            generator.Generate(db);
        }

        [TestMethod]
        public void GeneratorForeignKeyCompositeTestsDb2()
        {
            var db = new ForeignKeyCompositeSetForeignKeyToTestDb();
            var generator = new BimGenerator(DataDefinitionDocumenterTestsHelper.CreateTestContext(new DocumenterTests.TableCustomizer()), "ForeignKeyCompositeSetForeignKeyToTestDb");
            generator.Generate(db);
        }

        [TestMethod]
        public void GeneratorTabularRelationTestDb()
        {
            var db = new TabularRelationTestDb();

            var generator = new BimGenerator(DataDefinitionDocumenterTestsHelper.CreateTestContext(), "TabularRelationTestDb");
            generator.Generate(db);
        }
    }

    public class TabularRelationTestDb : DatabaseDeclaration
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