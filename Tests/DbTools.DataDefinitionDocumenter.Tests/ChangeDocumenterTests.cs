namespace FizzCode.DbTools.DataDefinitionDocumenter.Tests
{
    using System.Linq;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Tests;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ChangeDocumenterTests
    {
        [TestMethod]
        [LatestSqlVersions]
        public void AddFkTest(SqlEngineVersion version)
        {
            var ddOriginal = new TestDatabaseFk();
            ddOriginal.GetTable("Foreign").Properties.Remove(
                ddOriginal.GetTable("Foreign").Properties.OfType<ForeignKey>().First()
                );
            ddOriginal.SetVersions(version.GetTypeMapper());

            var ddWithFK = new TestDatabaseFk();
            ddWithFK.SetVersions(version.GetTypeMapper());

            var changeDocumenter = new ChangeDocumenter(DataDefinitionDocumenterTestsHelper.CreateTestChangeContext(version), version, "TestDatabaseFk", "TestDatabaseFk_AddFkTest");

            changeDocumenter.Document(ddOriginal, ddWithFK);
        }

        [TestMethod]
        [LatestSqlVersions]
            public void TempChangeFkTest(SqlEngineVersion version)
        {
            var ddOriginal = new TestDatabaseFkChange();
            ddOriginal.SetVersions(version.GetTypeMapper());

            var documnter = new Documenter(DataDefinitionDocumenterTestsHelper.CreateTestDocumenterContext(version), version, "TestDatabaseFkChange");
            documnter.Document(ddOriginal);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void ChangeFkTest(SqlEngineVersion version)
        {
            var ddOriginal = new TestDatabaseFkChange();
            ddOriginal.SetVersions(version.GetTypeMapper());

            var ddFkChanged = new TestDatabaseFkChange();
            ddFkChanged.GetTable("Foreign").Properties.Remove(
                ddFkChanged.GetTable("Foreign").Properties.OfType<ForeignKey>().First()
                );
            ddFkChanged.GetTable("Foreign").Columns["PrimaryId"].SetForeignKeyTo("Primary2", "FkChange");
            ddFkChanged.SetVersions(version.GetTypeMapper());

            var changeDocumenter = new ChangeDocumenter(DataDefinitionDocumenterTestsHelper.CreateTestChangeContext(version), version, "TestDatabaseFkChange", "TestDatabaseFkChange_ChangeFkTest");

            changeDocumenter.Document(ddOriginal, ddFkChanged);
        }

        [TestMethod]
        //[LatestSqlVersions]
        [SqlVersions("MsSql2016")]
        public void ChangeFkTestNotNullableToNullable(SqlEngineVersion version)
        {
            var ddOriginal = new TestDatabaseFk();
            ddOriginal.SetVersions(version.GetTypeMapper());

            var ddFkChanged = new TestDatabaseFk();
            ddFkChanged.SetVersions(version.GetTypeMapper());
            ddFkChanged.GetTable("Foreign")["PrimaryId"].Type.IsNullable = true;

            var changeDocumenter = new ChangeDocumenter(DataDefinitionDocumenterTestsHelper.CreateTestChangeContext(version), version, "TestDatabaseFkChange", "TestDatabaseFkChange_ChangeFkTestNotNullableToNullable");

            changeDocumenter.Document(ddOriginal, ddFkChanged);
        }

        [TestMethod]
        public void FkCheckNoCheckTest()
        {
            var version = MsSqlVersion.MsSql2016;

            var ddOriginal = new TestDatabaseFk();
            ddOriginal.SetVersions(version.GetTypeMapper());

            var ddFkChanged = new TestDatabaseFk();
            ddFkChanged.SetVersions(version.GetTypeMapper());

            var fk = ddFkChanged.GetTable("Foreign").Properties.OfType<ForeignKey>().First();
            fk.SqlEngineVersionSpecificProperties[version, "Nocheck"] = "false";

            var changeDocumenter = new ChangeDocumenter(DataDefinitionDocumenterTestsHelper.CreateTestChangeContext(version), version, "TestDatabaseFk", "TestDatabaseFk_FkCheckNoCheckTest");

            changeDocumenter.Document(ddOriginal, ddFkChanged);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void FkCompositeChangeFkNameChange(SqlEngineVersion version)
        {
            var ddOriginal = new ForeignKeyComposite();
            ddOriginal.SetVersions(version.GetTypeMapper());

            var ddFkChanged = new ForeignKeyComposite2();
            ddFkChanged.SetVersions(version.GetTypeMapper());

            var changeDocumenter = new ChangeDocumenter(DataDefinitionDocumenterTestsHelper.CreateTestChangeContext(version), version, "ForeignKeyComposite", "ForeignKeyComposite2_FkNameChange");

            changeDocumenter.Document(ddOriginal, ddFkChanged);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void FkCompositeChangeNoFkNameChange(SqlEngineVersion version)
        {
            var ddOriginal = new ForeignKeyComposite();
            ddOriginal.SetVersions(version.GetTypeMapper());

            var fkOriginal = ddOriginal
                .GetTable("TopOrdersPerCompany").Properties
                .OfType<ForeignKey>()
                .First(fk => fk.ForeignKeyColumns.Any(fkcm => fkcm.ForeignKeyColumn.Name == "Top2A"));

            fkOriginal.Name = "FK_TopOrdersPerCompany_2";

            var ddFkChanged = new ForeignKeyComposite2();
            ddFkChanged.SetVersions(version.GetTypeMapper());

            var changeDocumenter = new ChangeDocumenter(DataDefinitionDocumenterTestsHelper.CreateTestChangeContext(version), version, "ForeignKeyComposite", "ForeignKeyComposite2_NoFkNameChange");

            changeDocumenter.Document(ddOriginal, ddFkChanged);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void UniqueConstraintChange(SqlEngineVersion version)
        {
            var ddOriginal = new TestDatabaseUniqueConstraint();
            ddOriginal.SetVersions(version.GetTypeMapper());

            var ddFkChanged = new TestDatabaseUniqueConstraint2();
            ddFkChanged.SetVersions(version.GetTypeMapper());

            var changeDocumenter = new ChangeDocumenter(DataDefinitionDocumenterTestsHelper.CreateTestChangeContext(version), version, "TestDatabaseUniqueConstraint", "TestDatabaseUniqueConstraint2");
            changeDocumenter.Document(ddOriginal, ddFkChanged);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void RemoveTableTest(SqlEngineVersion version)
        {
            var ddOriginal = new TestDatabaseSimple();
            ddOriginal.SetVersions(version.GetTypeMapper());
            DatabaseMigratorTests.AddTable(ddOriginal);

            var dd = new TestDatabaseSimple();
            dd.SetVersions(version.GetTypeMapper());

            var changeDocumenter = new ChangeDocumenter(DataDefinitionDocumenterTestsHelper.CreateTestChangeContext(version), version, "TestDatabaseSimple", "TestDatabaseSimpleRemoveTableTest");
            changeDocumenter.Document(ddOriginal, dd);
        }
    }
}
