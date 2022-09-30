namespace FizzCode.DbTools.DataDefinitionDocumenter.Tests
{
    using System.Linq;
    using FizzCode.DbTools.DataDefinition.Base;
    using FizzCode.DbTools.DataDefinition.MsSql2016;
    using FizzCode.DbTools.DataDefinition.Oracle12c;
    using FizzCode.DbTools.DataDefinition.Tests;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ChangeDocumenterTests : ComparerTestsBase
    {
        private static void Document(SqlEngineVersion version, DatabaseDefinitions dds)
        {
            var changeDocumenter = new ChangeDocumenter(DataDefinitionDocumenterTestsHelper.CreateTestChangeContext(version), version, dds.DbNameOriginal, dds.DbNameNew);

            changeDocumenter.Document(dds.Original, dds.New);
        }

        [TestMethod]
        [LatestSqlVersions]
        public override void Fk_Add(SqlEngineVersion version)
        {
            var dds = Fk_Add_Dds(version);
            Document(version, dds);
        }

        [TestMethod]
        [LatestSqlVersions]
        public override void Fk_Remove(SqlEngineVersion version)
        {
            var dds = Fk_Remove_Dds(version);
            Document(version, dds);
        }

        [TestMethod]
        [LatestSqlVersions]
        public override void Column_Add(SqlEngineVersion version)
        {
            var dds = Column_Add_Dds(version);
            Document(version, dds);
        }

        [TestMethod]
        [LatestSqlVersions]
        public override void Column_Add2(SqlEngineVersion version)
        {
            var dds = Column_Add2_Dds(version);
            Document(version, dds);
        }

        [TestMethod]
        [LatestSqlVersions]
        public override void Column_Remove(SqlEngineVersion version)
        {
            var dds = Column_Remove_Dds(version);
            Document(version, dds);
        }

        [TestMethod]
        [LatestSqlVersions]
        public override void Column_Remove2(SqlEngineVersion version)
        {
            var dds = Column_Remove2_Dds(version);
            Document(version, dds);
        }

        [TestMethod]
        [LatestSqlVersions]
        public override void Table_Add(SqlEngineVersion version)
        {
            var dds = Table_Add_Dds(version);
            Document(version, dds);
        }

        [TestMethod]
        [LatestSqlVersions]
        public override void Table_Remove(SqlEngineVersion version)
        {
            var dds = Table_Remove_Dds(version);
            Document(version, dds);
        }

        [TestMethod]
        [LatestSqlVersions]
        public override void Pk_Add(SqlEngineVersion version)
        {
            var dds = Pk_Add_Dds(version);
            Document(version, dds);
        }

        [TestMethod]
        [LatestSqlVersions]
        public override void Identity_Change(SqlEngineVersion version)
        {
            var dds = Identity_Change_Dds(version);
            Document(version, dds);
        }

        [TestMethod]
        [LatestSqlVersions]
        public override void Column_Change_Length(SqlEngineVersion version)
        {
            TestHelper.CheckFeature(version, "ColumnLength");

            var dds = Column_Change_Length_Dds(version);
            Document(version, dds);
        }

        [TestMethod]
        [LatestSqlVersions]
        public override void Column_Change_Nullable(SqlEngineVersion version)
        {
            var dds = Column_Change_Nullable_Dds(version);
            Document(version, dds);
        }

        [TestMethod]
        [LatestSqlVersions]
        public override void Column_Change2_Length(SqlEngineVersion version)
        {
            TestHelper.CheckFeature(version, "ColumnLength");

            var dds = Column_Change2_Length_Dds(version);
            Document(version, dds);
        }

        [TestMethod]
        [SqlVersions(nameof(MsSql2016), nameof(Oracle12c))]
        public override void Column_Change_DbType(SqlEngineVersion version)
        {
            var dds = Column_Change_DbType_Dds(version);
            Document(version, dds);
        }

        [TestMethod]
        [SqlVersions(nameof(MsSql2016), nameof(Oracle12c))]
        public override void Column_Change_DbTypeAndLengthAndIsNullable(SqlEngineVersion version)
        {
            var dds = Column_Change_DbTypeAndLengthAndIsNullable_Dds(version);
            Document(version, dds);
        }

        [TestMethod]
        [LatestSqlVersions]
        public override void Fk_Change(SqlEngineVersion version)
        {
            var dds = Fk_Change_Dds(version);
            Document(version, dds);
        }

        [TestMethod]
        [LatestSqlVersions]
        public override void Column_Change_NotNullableToNullable_WithFk(SqlEngineVersion version)
        {
            var dds = Column_Change_NotNullableToNullable_WithFk_Dds(version);
            Document(version, dds);
        }

        [TestMethod]
        public void FkCheckNoCheckTest()
        {
            var version = MsSqlVersion.MsSql2016;

            var ddOriginal = new TestDatabaseFk();

            var ddFkChanged = new TestDatabaseFk();

            var fk = ddFkChanged.GetTable("Foreign").Properties.OfType<ForeignKey>().First();
            fk.SqlEngineVersionSpecificProperties[version, "Nocheck"] = "false";

            var changeDocumenter = new ChangeDocumenter(DataDefinitionDocumenterTestsHelper.CreateTestChangeContext(version), version, "TestDatabaseFk", "TestDatabaseFk_FkCheckNoCheckTest");

            changeDocumenter.Document(ddOriginal, ddFkChanged);
        }

        [TestMethod]
        [LatestSqlVersions]
        public override void Fk_Change_Composite_NameChange(SqlEngineVersion version)
        {
            var dds = Fk_Change_Composite_NameChange_Dds(version);
            Document(version, dds);
        }

        [TestMethod]
        [LatestSqlVersions]
        public override void Fk_Change_Composite_NoNameChange(SqlEngineVersion version)
        {
            var dds = Fk_Change_Composite_NoNameChange_Dds(version);
            Document(version, dds);
        }

        [TestMethod]
        [LatestSqlVersions]
        public override void UniqueConstraint_Change(SqlEngineVersion version)
        {
            var dds = UniqueConstraint_Change_Dds(version);
            Document(version, dds);
        }

        [TestMethod]
        [LatestSqlVersions]
        public override void UniqueConstraint_Change_NewColumn(SqlEngineVersion version)
        {
            var dds = UniqueConstraint_Change_NewColumn_Dds(version);
            Document(version, dds);
        }

        [TestMethod]
        [LatestSqlVersions]
        public override void UniqueConstraint_Change_NewColumn_UcName(SqlEngineVersion version)
        {
            var dds = UniqueConstraint_Change_NewColumn_Dds(version);
            Document(version, dds);
        }

        [TestMethod]
        [LatestSqlVersions]
        public override void Index_Add(SqlEngineVersion version)
        {
            var dds = Index_Add_Dds(version);
            Document(version, dds);
        }

        [TestMethod]
        [LatestSqlVersions]
        public override void Index_Change(SqlEngineVersion version)
        {
            var dds = Index_Change_Dds(version);
            Document(version, dds);
        }
    }
}
