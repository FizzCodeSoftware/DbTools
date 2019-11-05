namespace FizzCode.DbTools.DataDefinitionDocumenter.Tests
{
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Tests;
    using FizzCode.DbTools.Tabular;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BimGeneratorTests
    {
        [TestMethod]
        public void GeneratorTestDatabaseFks()
        {
            var db = new TestDatabaseFks();

            var generator = new BimGenerator(new DocumenterSettings(), TestHelper.GetDefaultTestSettings(SqlDialect.MsSql), "TestDatabaseFks", new DocumenterTests.TableCustomizer());
            generator.Generate(db);
        }

        [TestMethod]
        public void GeneratorForeignKeyCompositeTestsDb()
        {
            var db = new ForeignKeyCompositeTestsDb();

            var generator = new BimGenerator(new DocumenterSettings(), TestHelper.GetDefaultTestSettings(SqlDialect.MsSql), "TestDatabaseFks");
            generator.Generate(db);
        }

        [TestMethod]
        public void GeneratorForeignKeyCompositeTestsDb1()
        {
            var db = new ForeignKeyCompositeTestsDb();

            var generator = new BimGenerator(new DocumenterSettings(), TestHelper.GetDefaultTestSettings(SqlDialect.MsSql), "ForeignKeyCompositeTestsDb");
            generator.Generate(db);
        }

        [TestMethod]
        public void GeneratorForeignKeyCompositeTestsDb2()
        {
            var db = new ForeignKeyCompositeSetForeignKeyToTestDb();
            var generator = new BimGenerator(new DocumenterSettings(), TestHelper.GetDefaultTestSettings(SqlDialect.MsSql), "ForeignKeyCompositeSetForeignKeyToTestDb", new DocumenterTests.TableCustomizer());
            generator.Generate(db);
        }

        [TestMethod]
        public void GeneratorTabularRelationTestDb()
        {
            var db = new TabularRelationTestDb();

            var generator = new BimGenerator(new DocumenterSettings(), TestHelper.GetDefaultTestSettings(SqlDialect.MsSql), "TabularRelationTestDb");
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