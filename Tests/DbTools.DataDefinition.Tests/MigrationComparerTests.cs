namespace FizzCode.DbTools.DataDefinition.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.DataDeclaration;
    using FizzCode.DbTools.DataDefinition.Base;
    using FizzCode.DbTools.DataDefinition.Base.Migration;
    using FizzCode.DbTools.DataDefinition.Generic1;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MigrationComparerTests : ComparerTestsBase
    {
        [TestMethod]
        [LatestSqlVersions(true)]
        public override void Table_Add(SqlEngineVersion version)
        {
            var dds = Table_Add_Dds(version);
            var changes = Compare(version, dds);
            var first = (TableNew)changes[0];
            Assert.AreEqual((SchemaAndTableName)"NewTableToMigrate", first.SchemaAndTableName);
        }

        public static void AddTable(TestDatabaseSimple dd)
        {
            var newTable = new SqlTable
            {
                SchemaAndTableName = "NewTableToMigrate"
            };
            newTable.AddInt32("Id", false).SetPK().SetIdentity();

            new PrimaryKeyNamingDefaultStrategy().SetPrimaryKeyName(newTable.Properties.OfType<PrimaryKey>().First());

            newTable.AddNVarChar("Name", 100);

            dd.AddTable(newTable);
        }

        [TestMethod]
        [LatestSqlVersions(true)]
        public override void Table_Remove(SqlEngineVersion version)
        {
            var dds = Table_Remove_Dds(version);
            var changes = Compare(version, dds);

            Assert.AreEqual(1, changes.Count);
            var first = (TableDelete)changes[0];
            Assert.AreEqual((SchemaAndTableName)"NewTableToMigrate", first.SchemaAndTableName);
        }

        [TestMethod]
        [LatestSqlVersions(true)]
        public override void Column_Remove(SqlEngineVersion version)
        {
            var dds = Column_Remove_Dds(version);
            var changes = Compare(version, dds);
            var first = (ColumnDelete)changes[0];
            Assert.AreEqual("Name", first.Name);
        }

        [TestMethod]
        [LatestSqlVersions(true)]
        public override void Column_Add(SqlEngineVersion version)
        {
            var dds = Column_Add_Dds(version);
            var changes = Compare(version, dds);

            Assert.AreEqual(1, changes.Count);
            var first = (ColumnNew)changes[0];
            Assert.AreEqual("Name2", first.Name);
        }

        [TestMethod]
        [LatestSqlVersions(true)]
        public override void Column_Add2(SqlEngineVersion version)
        {
            var dds = Column_Add2_Dds(version);
            var changes = Compare(version, dds);

            Assert.AreEqual(2, changes.Count);
            var first = (ColumnNew)changes[0];
            Assert.AreEqual("Name2", first.Name);
            var second = (ColumnNew)changes[1];
            Assert.AreEqual("Name3", second.Name);
            Assert.AreEqual(true, second.Type.IsNullable);
        }

        [TestMethod]
        [LatestSqlVersions(true)]
        public override void Column_Change_Length(SqlEngineVersion version)
        {
            TestHelper.CheckFeature(version, "ColumnLength");

            var dds = Column_Change_Length_Dds(version);
            var changes = Compare(version, dds);

            Assert.AreEqual(1, changes.Count);
            var first = (ColumnChange)changes[0];
            Assert.AreEqual(100, dds.Original.GetTable("Company")["Name"].Type.Length);
            Assert.AreEqual(100, first.SqlColumn.Type.Length);
            Assert.AreEqual(101, first.NewNameAndType.Type.Length);
        }

        [TestMethod]
        [LatestSqlVersions(true)]
        public override void Column_Change_Nullable(SqlEngineVersion version)
        {
            var dds = Column_Change_Nullable_Dds(version);
            var changes = Compare(version, dds);

            Assert.AreEqual(1, changes.Count);
            var first = (ColumnChange)changes[0];
            Assert.AreEqual(false, dds.Original.GetTable("Company")["Name"].Type.IsNullable);
            Assert.AreEqual(false, first.SqlColumn.Type.IsNullable);
            Assert.AreEqual(true, first.NewNameAndType.Type.IsNullable);
        }

        [TestMethod]
        [LatestSqlVersions(true)]
        public override void Column_Change2_Length(SqlEngineVersion version)
        {
            TestHelper.CheckFeature(version, "ColumnLength");

            var dds = Column_Change2_Length_Dds(version);
            var changes = Compare(version, dds);

            Assert.AreEqual(2, changes.Count);
            var first = (ColumnChange)changes[0];

            Assert.AreEqual(100, dds.Original.GetTable("Company")["Name"].Type.Length);

            Assert.AreEqual(100, first.SqlColumn.Type.Length);
            Assert.AreEqual(101, first.NewNameAndType.Type.Length);

            var second = (ColumnChange)changes[0];

            Assert.AreEqual(100, second.SqlColumn.Type.Length);
            Assert.AreEqual(101, second.NewNameAndType.Type.Length);
        }

        [TestMethod]
        [LatestSqlVersions(true)]
        public override void Column_Change_DbType(SqlEngineVersion version)
        {
            // No different types for these tests in SqLite3
            if (version == SqLiteVersion.SqLite3)
                return;

            var dds = Column_Change_DbType_Dds(version);
            var changes = Compare(version, dds);

            Assert.AreEqual(1, changes.Count);
            var first = (ColumnChange)changes[0];

            if (version == MsSqlVersion.MsSql2016)
            {
                Assert.IsTrue(dds.Original.GetTable("Company")["Name"].Type.SqlTypeInfo is MsSql2016.SqlNVarChar);
                Assert.IsTrue(dds.New.GetTable("Company")["Name"].Type.SqlTypeInfo is MsSql2016.SqlNChar);
                Assert.IsTrue(first.SqlColumn.Type.SqlTypeInfo is MsSql2016.SqlNVarChar);
                Assert.IsTrue(first.NewNameAndType.Type.SqlTypeInfo is MsSql2016.SqlNChar);
            }
            else if (version == OracleVersion.Oracle12c)
            {
                Assert.IsTrue(dds.Original.GetTable("Company")["Name"].Type.SqlTypeInfo is Oracle12c.SqlNVarChar2);
                Assert.IsTrue(dds.New.GetTable("Company")["Name"].Type.SqlTypeInfo is Oracle12c.SqlNChar);
                Assert.IsTrue(first.SqlColumn.Type.SqlTypeInfo is Oracle12c.SqlNVarChar2);
                Assert.IsTrue(first.NewNameAndType.Type.SqlTypeInfo is Oracle12c.SqlNChar);
            }
        }

        [TestMethod]
        [LatestSqlVersions(true)]
        public override void Column_Change_DbTypeAndLengthAndIsNullable(SqlEngineVersion version)
        {
            // No different types for these tests in SqLite3
            if (version == SqLiteVersion.SqLite3)
                return;

            var dds = Column_Change_DbTypeAndLengthAndIsNullable_Dds(version);
            var changes = Compare(version, dds);

            Assert.AreEqual(1, changes.Count);
            var first = (ColumnChange)changes[0];

            if (version == MsSqlVersion.MsSql2016)
            {
                Assert.IsTrue(dds.Original.GetTable("Company")["Name"].Type.SqlTypeInfo is MsSql2016.SqlNVarChar);
                Assert.IsTrue(dds.New.GetTable("Company")["Name"].Type.SqlTypeInfo is MsSql2016.SqlNChar);
                Assert.IsTrue(first.SqlColumn.Type.SqlTypeInfo is MsSql2016.SqlNVarChar);
                Assert.IsTrue(first.NewNameAndType.Type.SqlTypeInfo is MsSql2016.SqlNChar);
            }
            else if (version == OracleVersion.Oracle12c)
            {
                Assert.IsTrue(dds.Original.GetTable("Company")["Name"].Type.SqlTypeInfo is Oracle12c.SqlNVarChar2);
                Assert.IsTrue(dds.New.GetTable("Company")["Name"].Type.SqlTypeInfo is Oracle12c.SqlNChar);
                Assert.IsTrue(first.SqlColumn.Type.SqlTypeInfo is Oracle12c.SqlNVarChar2);
                Assert.IsTrue(first.NewNameAndType.Type.SqlTypeInfo is Oracle12c.SqlNChar);
            }

            Assert.AreEqual(100, dds.Original.GetTable("Company")["Name"].Type.Length);
            Assert.AreEqual(101, dds.New.GetTable("Company")["Name"].Type.Length);
            Assert.AreEqual(100, first.SqlColumn.Type.Length);
            Assert.AreEqual(101, first.NewNameAndType.Type.Length);
            Assert.AreEqual(false, dds.Original.GetTable("Company")["Name"].Type.IsNullable);
            Assert.AreEqual(true, dds.New.GetTable("Company")["Name"].Type.IsNullable);
            Assert.AreEqual(false, first.SqlColumn.Type.IsNullable);
            Assert.AreEqual(true, first.NewNameAndType.Type.IsNullable);
        }

        [TestMethod]
        [LatestSqlVersions(true)]
        public override void Column_Remove2(SqlEngineVersion version)
        {
            var dds = Column_Remove2_Dds(version);
            var changes = Compare(version, dds);

            Assert.AreEqual(2, changes.Count);
            var first = (ColumnDelete)changes[0];
            Assert.AreEqual("Name", first.Name);
            var second = (ColumnDelete)changes[1];
            Assert.AreEqual("Name2", second.Name);
        }

        [TestMethod]
        [LatestSqlVersions(true)]
        public override void Index_Add(SqlEngineVersion version)
        {
            var dds = Index_Add_Dds(version);
            var changes = Compare(version, dds);

            Assert.AreEqual(1, changes.Count);
            _ = (IndexNew)changes[0];
        }

        [TestMethod]
        [LatestSqlVersions(true)]
        public override void Index_Change(SqlEngineVersion version)
        {
            var dds = Index_Change_Dds(version);
            var changes = Compare(version, dds);

            Assert.AreEqual(1, changes.Count);
            _ = (IndexChange)changes[0];
        }

        [TestMethod]
        [LatestSqlVersions(true)]
        public override void UniqueConstraint_Change(SqlEngineVersion version)
        {
            var dds = UniqueConstraint_Change_Dds(version);
            var changes = Compare(version, dds);

            Assert.AreEqual(2, changes.Count);
            _ = (UniqueConstraintDelete)changes[0];
            _ = (UniqueConstraintNew)changes[1];
        }

        [TestMethod]
        [LatestSqlVersions(true)]
        public override void UniqueConstraint_Change_NewColumn(SqlEngineVersion version)
        {
            var dds = UniqueConstraint_Change_NewColumn_Dds(version);
            var changes = Compare(version, dds);

            Assert.AreEqual(3, changes.Count);
            _ = (ColumnNew)changes[0];
            _ = (UniqueConstraintDelete)changes[1];
            _ = (UniqueConstraintNew)changes[2];
        }

        [TestMethod]
        [LatestSqlVersions(true)]
        public override void UniqueConstraint_Change_NewColumn_UcName(SqlEngineVersion version)
        {
            var dds = UniqueConstraint_Change_NewColumn_UcName_Dds(version);
            var changes = Compare(version, dds);

            Assert.AreEqual(2, changes.Count);
            _ = (ColumnNew)changes[0];
            _ = (UniqueConstraintChange)changes[1];
        }

        private static List<IMigration> Compare(SqlEngineVersion version, DatabaseDefinitions dds)
        {
            var comparer = new Comparer();
            var changes = comparer.Compare(dds.Original, dds.New);

            return changes;
        }

        [TestMethod]
        [LatestSqlVersions(true)]
        public override void Fk_Add(SqlEngineVersion version)
        {
            var dds = Fk_Add_Dds(version);
            var changes = Compare(version, dds);

            Assert.AreEqual(1, changes.Count);
            _ = (ForeignKeyNew)changes[0];
        }

        [TestMethod]
        [LatestSqlVersions(true)]
        public override void Fk_Remove(SqlEngineVersion version)
        {
            var dds = Fk_Remove_Dds(version);
            var changes = Compare(version, dds);

            Assert.AreEqual(1, changes.Count);
            _ = changes[0] as ForeignKeyDelete;
        }

        [TestMethod]
        [LatestSqlVersions(true)]
        public override void Fk_Change(SqlEngineVersion version)
        {
            var dds = Fk_Change_Dds(version);
            var changes = Compare(version, dds);

            Assert.AreEqual(1, changes.Count);
            _ = changes[0] as ForeignKeyChange;
        }

        [TestMethod]
        [LatestSqlVersions(true)]
        public override void Fk_Change_Composite_NameChange(SqlEngineVersion version)
        {
            var dds = Fk_Change_Composite_NameChange_Dds(version);
            var changes = Compare(version, dds);

            Assert.AreEqual(2, changes.Count);

            var fkDelete = changes[0] as ForeignKeyDelete;
            Assert.AreEqual("Top2A", fkDelete.ForeignKey.ForeignKeyColumns[0].ForeignKeyColumn.Name);
            Assert.AreEqual("Top2B", fkDelete.ForeignKey.ForeignKeyColumns[1].ForeignKeyColumn.Name);

            Assert.AreEqual(2, fkDelete.ForeignKey.ForeignKeyColumns.Count);

            var fkNew = changes[1] as ForeignKeyNew;
            Assert.AreEqual("Top2A", fkNew.ForeignKey.ForeignKeyColumns[0].ForeignKeyColumn.Name);
            Assert.AreEqual("Top2B", fkNew.ForeignKey.ForeignKeyColumns[1].ForeignKeyColumn.Name);
            Assert.AreEqual("Top2C", fkNew.ForeignKey.ForeignKeyColumns[2].ForeignKeyColumn.Name);

            Assert.AreEqual(3, fkNew.ForeignKey.ForeignKeyColumns.Count);
        }

        [TestMethod]
        [LatestSqlVersions(true)]
        public override void Fk_Change_Composite_NoNameChange(SqlEngineVersion version)
        {
            var dds = Fk_Change_Composite_NoNameChange_Dds(version);
            var changes = Compare(version, dds);

            Assert.AreEqual(1, changes.Count);

            var fkChange = changes[0] as ForeignKeyChange;

            Assert.AreEqual(2, fkChange.ForeignKey.ForeignKeyColumns.Count);

            Assert.AreEqual("Top2A", fkChange.ForeignKey.ForeignKeyColumns[0].ForeignKeyColumn.Name);
            Assert.AreEqual("Top2B", fkChange.ForeignKey.ForeignKeyColumns[1].ForeignKeyColumn.Name);

            Assert.AreEqual(3, fkChange.NewForeignKey.ForeignKeyColumns.Count);

            Assert.AreEqual("Top2A", fkChange.NewForeignKey.ForeignKeyColumns[0].ForeignKeyColumn.Name);
            Assert.AreEqual("Top2B", fkChange.NewForeignKey.ForeignKeyColumns[1].ForeignKeyColumn.Name);
            Assert.AreEqual("Top2C", fkChange.NewForeignKey.ForeignKeyColumns[2].ForeignKeyColumn.Name);
        }

        [TestMethod]
        [LatestSqlVersions(true)]
        public override void Column_Change_NotNullableToNullable_WithFk(SqlEngineVersion version)
        {
            var dds = Column_Change_NotNullableToNullable_WithFk_Dds(version);
            var changes = Compare(version, dds);

            Assert.AreEqual(2, changes.Count);
            _ = changes[0] as ColumnChange;
            _ = changes[1] as ForeignKeyChange;
        }

        [TestMethod]
        [LatestSqlVersions(true)]
        public override void Pk_Add(SqlEngineVersion version)
        {
            var dds = Pk_Add_Dds(version);
            var changes = Compare(version, dds);

            Assert.AreEqual(1, changes.Count);
            _ = changes[0] as PrimaryKeyNew;
        }

        [TestMethod]
        [LatestSqlVersions(true)]
        public override void Identity_Change(SqlEngineVersion version)
        {
            var dds = Identity_Change_Dds(version);
            var changes = Compare(version, dds);

            Assert.AreEqual(1, changes.Count);
            var columnChange = (ColumnChange)changes[0];
            var _ = columnChange.SqlColumnPropertyMigrations[0] as IdentityChange;
        }
    }
}