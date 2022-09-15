namespace FizzCode.DbTools.DataDefinitionDocumenter.Tests
{
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Base;
    using FizzCode.DbTools.DataDefinition.Generic1;
    using FizzCode.DbTools.DataDefinition.Tests;
    using FizzCode.DbTools.Tabular;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public static class DataDefinitionDocumenterTestsHelper
    {
        public static DocumenterContext CreateTestDocumenterContext(SqlEngineVersion version, ITableCustomizer customizer = null)
        {
            var context = new DocumenterContext
            {
                Settings = TestHelper.GetDefaultTestSettings(version),
                DocumenterSettings = new DocumenterSettings(),
                Customizer = customizer ?? new EmptyTableCustomizer(),
                Logger = TestHelper.CreateLogger()
            };

            return context;
        }

        public static ChangeDocumenterContext CreateTestChangeContext(SqlEngineVersion version, ITableCustomizer customizerOriginal = null, ITableCustomizer customizerNew = null)
        {
            var documenterContext = CreateTestDocumenterContext(version, null);

            var changeDocumenterContext = new ChangeDocumenterContext
            {
                Settings = documenterContext.Settings,
                DocumenterSettings = documenterContext.DocumenterSettings,
                Logger = documenterContext.Logger,
                CustomizerOriginal = customizerOriginal ?? new EmptyTableCustomizer(),
                CustomizerNew = customizerNew ?? new EmptyTableCustomizer()
            };

            return changeDocumenterContext;
        }

        public static GeneratorContext CreateTestGeneratorContext(SqlEngineVersion version, ITableCustomizer customizer = null)
        {
            var context = new GeneratorContext
            {
                Settings = TestHelper.GetDefaultTestSettings(version),
                GeneratorSettings = new GeneratorSettings(),
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
        public void GeneratorTestDatabaseFks(SqlEngineVersion version)
        {
            var db = new TestDatabaseFks();

            var generator = new BimGenerator(DataDefinitionDocumenterTestsHelper.CreateTestGeneratorContext(version, new DocumenterTests.TableCustomizer()), version, "TestDatabaseFks");
            generator.Generate(db);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void GeneratorForeignKeyComposite(SqlEngineVersion version)
        {
            var db = new ForeignKeyComposite();

            var generator = new BimGenerator(DataDefinitionDocumenterTestsHelper.CreateTestGeneratorContext(version), version, "TestDatabaseFks");
            generator.Generate(db);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void GeneratorForeignKeyComposite1(SqlEngineVersion version)
        {
            var db = new ForeignKeyComposite();

            var generator = new BimGenerator(DataDefinitionDocumenterTestsHelper.CreateTestGeneratorContext(version), version, "ForeignKeyComposite");
            generator.Generate(db);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void GeneratorForeignKeyComposite2(SqlEngineVersion version)
        {
            var db = new ForeignKeyCompositeSetForeignKeyTo();
            var generator = new BimGenerator(DataDefinitionDocumenterTestsHelper.CreateTestGeneratorContext(version, new DocumenterTests.TableCustomizer()), version, "ForeignKeyCompositeSetForeignKeyTo");
            generator.Generate(db);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void GeneratorTabularRelation(SqlEngineVersion version)
        {
            var db = new TabularRelation();

            var generator = new BimGenerator(DataDefinitionDocumenterTestsHelper.CreateTestGeneratorContext(version), version, "TabularRelation");
            generator.Generate(db);
        }
    }

    public class TabularRelation : TestDatabaseDeclaration
    {
        public SqlTable KeyTable { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddNVarChar("Value", 100);
        });

        public SqlTable ReferringTableA { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddInt32("KeyTableId1").SetForeignKeyToTable("KeyTable").AddTabularRelation("dbo", "KeyTable", "Id", "Relation1");
            table.AddInt32("KeyTableId2").SetForeignKeyToTable("KeyTable").AddTabularRelation("dbo", "KeyTable", "Id", "Relation2");
        });

        public SqlTable ReferringTableB { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddInt32("KeyTableId1").SetForeignKeyToTable("KeyTable").AddTabularRelation("dbo", "KeyTable", "Id", "Relation1");
            table.AddInt32("KeyTableId2").SetForeignKeyToTable("KeyTable").AddTabularRelation("dbo", "KeyTable", "Id", "Relation2");
        });
    }
}