namespace FizzCode.DbTools.DataDefinitionDocumenter.Tests
{
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Common.Logger;
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
        public void GeneratorForeignKeyComposite()
        {
            var db = new ForeignKeyComposite();

            var generator = new BimGenerator(DataDefinitionDocumenterTestsHelper.CreateTestContext(), "TestDatabaseFks");
            generator.Generate(db);
        }

        [TestMethod]
        public void GeneratorForeignKeyComposite1()
        {
            var db = new ForeignKeyComposite();

            var generator = new BimGenerator(DataDefinitionDocumenterTestsHelper.CreateTestContext(), "ForeignKeyComposite");
            generator.Generate(db);
        }

        [TestMethod]
        public void GeneratorForeignKeyComposite2()
        {
            var db = new ForeignKeyCompositeSetForeignKeyTo();
            var generator = new BimGenerator(DataDefinitionDocumenterTestsHelper.CreateTestContext(new DocumenterTests.TableCustomizer()), "ForeignKeyCompositeSetForeignKeyTo");
            generator.Generate(db);
        }

        [TestMethod]
        public void GeneratorTabularRelation()
        {
            var db = new TabularRelation();

            var generator = new BimGenerator(DataDefinitionDocumenterTestsHelper.CreateTestContext(), "TabularRelation");
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