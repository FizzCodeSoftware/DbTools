namespace FizzCode.DbTools.DataDefinition.Tests
{
    using System.Linq;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Base;
    using FizzCode.DbTools.DataDefinition.Factory;
    using FizzCode.DbTools.DataDefinition.Generic1;

    public abstract class ComparerTestsBase
    {
        protected class DatabaseDefinitions
        {
            public DatabaseDefinitions(DatabaseDefinition originalDd, DatabaseDefinition newDd, string dbNameOriginal, string dbNameNew)
            {
                Original = originalDd;
                New = newDd;
                DbNameOriginal = dbNameOriginal;
                DbNameNew = dbNameNew;
            }

            public DatabaseDefinition Original { get; }
            public DatabaseDefinition New { get; }

            public string DbNameOriginal { get; }
            public string DbNameNew { get; }
        }

        public abstract void Table_Add(SqlEngineVersion version);
        protected static DatabaseDefinitions Table_Add_Dds(SqlEngineVersion version)
        {
            var ddOriginal = new TestDatabaseSimple();
            ddOriginal.SetVersions(version.GetTypeMapper());

            var ddWithNewTable = new TestDatabaseSimple();
            ddWithNewTable.SetVersions(version.GetTypeMapper());
            AddTable(ddWithNewTable);

            return new DatabaseDefinitions(ddOriginal, ddWithNewTable, "TestDatabaseSimple", "TestDatabaseSimple_Table_Add");
        }

        private static void AddTable(TestDatabaseSimple dd)
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

        public abstract void Table_Remove(SqlEngineVersion version);
        protected static DatabaseDefinitions Table_Remove_Dds(SqlEngineVersion version)
        {
            var ddOriginal = new TestDatabaseSimple();
            ddOriginal.SetVersions(version.GetTypeMapper());
            AddTable(ddOriginal);

            var ddWithoutNewTable = new TestDatabaseSimple();
            ddWithoutNewTable.SetVersions(version.GetTypeMapper());

            return new DatabaseDefinitions(ddOriginal, ddWithoutNewTable, "TestDatabaseSimple", "TestDatabaseSimple_Table_Remove");
        }

        public abstract void Fk_Add(SqlEngineVersion version);
        protected static DatabaseDefinitions Fk_Add_Dds(SqlEngineVersion version)
        {
            var ddOriginal = new TestDatabaseFk();
            ddOriginal.GetTable("Foreign").Properties.Remove(
                ddOriginal.GetTable("Foreign").Properties.OfType<ForeignKey>().First()
                );
            ddOriginal.SetVersions(version.GetTypeMapper());

            var ddWithFK = new TestDatabaseFk();
            ddWithFK.SetVersions(version.GetTypeMapper());

            return new DatabaseDefinitions(ddOriginal, ddWithFK, "TestDatabaseFk", "TestDatabaseFk_Fk_Add");
        }

        public abstract void Fk_Remove(SqlEngineVersion version);
        protected static DatabaseDefinitions Fk_Remove_Dds(SqlEngineVersion version)
        {
            var ddOriginal = new TestDatabaseFk();
            ddOriginal.SetVersions(version.GetTypeMapper());

            var ddFKRemoved = new TestDatabaseFk();
            ddFKRemoved.GetTable("Foreign").Properties.Remove(
                ddFKRemoved.GetTable("Foreign").Properties.OfType<ForeignKey>().First()
                );
            ddFKRemoved.SetVersions(version.GetTypeMapper());

            return new DatabaseDefinitions(ddOriginal, ddFKRemoved, "TestDatabaseFk", "TestDatabaseFk_Fk_Remove");
        }

        public abstract void Fk_Change(SqlEngineVersion version);
        protected static DatabaseDefinitions Fk_Change_Dds(SqlEngineVersion version)
        {
            var ddOriginal = new TestDatabaseFkChange();
            ddOriginal.SetVersions(version.GetTypeMapper());

            var ddFkChanged = new TestDatabaseFkChange();
            ddFkChanged.GetTable("Foreign").Properties.Remove(
                ddFkChanged.GetTable("Foreign").Properties.OfType<ForeignKey>().First()
                );
            ddFkChanged.GetTable("Foreign").Columns["PrimaryId"].SetForeignKeyToTable("Primary2", "FkChange");
            ddFkChanged.SetVersions(version.GetTypeMapper());

            return new DatabaseDefinitions(ddOriginal, ddFkChanged, "TestDatabaseFkChange", "TestDatabaseFkChange_Fk_Change");
        }

        public abstract void Fk_Change_Composite_NameChange(SqlEngineVersion version);
        protected static DatabaseDefinitions Fk_Change_Composite_NameChange_Dds(SqlEngineVersion version)
        {
            var ddOriginal = new ForeignKeyComposite();
            ddOriginal.GetTable("Order").AddInt32("LineNumber2");
            ddOriginal.GetTable("Order").AddUniqueConstraintWithName("UQ_Order_OrderHeaderId_LineNumber_LineNumber2", "OrderHeaderId", "LineNumber", "LineNumber2");
            ddOriginal.GetTable("TopOrdersPerCompany").AddInt32("Top2C");
            ddOriginal.SetVersions(version.GetTypeMapper());

            var ddFkChanged = new ForeignKeyComposite2();
            ddFkChanged.SetVersions(version.GetTypeMapper());

            return new DatabaseDefinitions(ddOriginal, ddFkChanged, "ForeignKeyComposite", "ForeignKeyComposite2_Fk_Change_Composite_NameChange");
        }

        public abstract void Fk_Change_Composite_NoNameChange(SqlEngineVersion version);
        protected static DatabaseDefinitions Fk_Change_Composite_NoNameChange_Dds(SqlEngineVersion version)
        {
            var ddOriginal = new ForeignKeyComposite();
            ddOriginal.GetTable("Order").AddInt32("LineNumber2");
            ddOriginal.GetTable("Order").AddUniqueConstraintWithName("UQ_Order_OrderHeaderId_LineNumber_LineNumber2", "OrderHeaderId", "LineNumber", "LineNumber2");
            ddOriginal.GetTable("TopOrdersPerCompany").AddInt32("Top2C");
            ddOriginal.SetVersions(version.GetTypeMapper());

            var fkOriginal = ddOriginal
                .GetTable("TopOrdersPerCompany").Properties
                .OfType<ForeignKey>()
                .First(fk => fk.ForeignKeyColumns.Any(fkcm => fkcm.ForeignKeyColumn.Name == "Top2A"));

            fkOriginal.Name = "FK_TopOrdersPerCompany_2";

            var ddFkChanged = new ForeignKeyComposite2();
            ddFkChanged.SetVersions(version.GetTypeMapper());

            return new DatabaseDefinitions(ddOriginal, ddFkChanged, "ForeignKeyComposite", "ForeignKeyComposite2_Fk_Change_Composite_NoNameChange");
        }

        public abstract void Column_Change_NotNullableToNullable_WithFk(SqlEngineVersion version);
        protected static DatabaseDefinitions Column_Change_NotNullableToNullable_WithFk_Dds(SqlEngineVersion version)
        {
            var ddOriginal = new TestDatabaseFk();
            ddOriginal.SetVersions(version.GetTypeMapper());

            var ddFkChanged = new TestDatabaseFk();
            ddFkChanged.SetVersions(version.GetTypeMapper());
            ddFkChanged.GetTable("Foreign")["PrimaryId"].Type.IsNullable = true;

            return new DatabaseDefinitions(ddOriginal, ddFkChanged, "ForeignKeyComposite", "ForeignKeyComposite2_Column_Change_NotNullableToNullable_WithFk");
        }

        public abstract void Column_Add(SqlEngineVersion version);
        protected static DatabaseDefinitions Column_Add_Dds(SqlEngineVersion version)
        {
            var ddOriginal = new TestDatabaseSimple();
            ddOriginal.SetVersions(version.GetTypeMapper());

            var ddWithNewColumn = new TestDatabaseSimple();
            ddWithNewColumn.SetVersions(version.GetTypeMapper());

            ddWithNewColumn.GetTable("Company").AddVarChar("Name2", 100);

            return new DatabaseDefinitions(ddOriginal, ddWithNewColumn, "TestDatabaseSimple", "TestDatabaseSimple_Column_Add");
        }

        public abstract void Column_Add2(SqlEngineVersion version);
        protected static DatabaseDefinitions Column_Add2_Dds(SqlEngineVersion version)
        {
            var ddOriginal = new TestDatabaseSimple();
            ddOriginal.SetVersions(version.GetTypeMapper());

            var ddWithNewColumn = new TestDatabaseSimple();
            ddWithNewColumn.SetVersions(version.GetTypeMapper());

            ddWithNewColumn.GetTable("Company").AddVarChar("Name2", 100);
            ddWithNewColumn.GetTable("Company").AddVarChar("Name3", 100, true);

            return new DatabaseDefinitions(ddOriginal, ddWithNewColumn, "TestDatabaseSimple", "TestDatabaseSimple_Column_Add2");
        }

        public abstract void Column_Remove(SqlEngineVersion version);
        protected static DatabaseDefinitions Column_Remove_Dds(SqlEngineVersion version)
        {
            var ddOriginal = new TestDatabaseSimple();
            ddOriginal.SetVersions(version.GetTypeMapper());

            var ddColumnRemoved = new TestDatabaseSimple();
            ddColumnRemoved.SetVersions(version.GetTypeMapper());

            ddColumnRemoved.GetTable("Company").Columns.Remove("Name");

            return new DatabaseDefinitions(ddOriginal, ddColumnRemoved, "TestDatabaseSimple", "TestDatabaseSimple_Column_Remove");
        }

        public abstract void Column_Remove2(SqlEngineVersion version);
        protected static DatabaseDefinitions Column_Remove2_Dds(SqlEngineVersion version)
        {
            var ddOriginal = new TestDatabaseSimple();
            ddOriginal.SetVersions(version.GetTypeMapper());
            ddOriginal.GetTable("Company").AddNVarChar("Name2", 100);

            var ddColumnsRemoved = new TestDatabaseSimple();
            ddColumnsRemoved.SetVersions(version.GetTypeMapper());

            ddColumnsRemoved.GetTable("Company").Columns.Remove("Name");
            ddColumnsRemoved.GetTable("Company").Columns.Remove("Name2");

            return new DatabaseDefinitions(ddOriginal, ddColumnsRemoved, "TestDatabaseSimple", "TestDatabaseSimple_Column_Remove2");
        }

        public abstract void Pk_Add(SqlEngineVersion version);
        protected static DatabaseDefinitions Pk_Add_Dds(SqlEngineVersion version)
        {
            var ddOriginal = new TestDatabaseSimple();
            ddOriginal.GetTable("Company").Columns["Id"].Properties.Remove(
                ddOriginal.GetTable("Company").Columns["Id"].Properties.OfType<Identity>().First()
                );
            ddOriginal.GetTable("Company").Properties.Remove(
                ddOriginal.GetTable("Company").Properties.OfType<PrimaryKey>().First()
                );
            ddOriginal.SetVersions(version.GetTypeMapper());

            var ddWithPk = new TestDatabaseSimple();
            ddWithPk.SetVersions(version.GetTypeMapper());
            ddWithPk.GetTable("Company").Columns["Id"].Properties.Remove(
                ddWithPk.GetTable("Company").Columns["Id"].Properties.OfType<Identity>().First()
                );

            return new DatabaseDefinitions(ddOriginal, ddWithPk, "TestDatabaseSimple", "TestDatabaseSimple_Pk_Add");
        }

        public abstract void Identity_Change(SqlEngineVersion version);
        protected static DatabaseDefinitions Identity_Change_Dds(SqlEngineVersion version)
        {
            var ddOriginal = new TestDatabaseSimple();
            ddOriginal.SetVersions(version.GetTypeMapper());

            var ddIdentityChanged = new TestDatabaseSimple();
            ddIdentityChanged.SetVersions(version.GetTypeMapper());
            var identity = ddIdentityChanged.Company.Columns["Id"].Properties.OfType<Identity>().First();
            identity.Increment = 2;

            return new DatabaseDefinitions(ddOriginal, ddIdentityChanged, "TestDatabaseSimple", "TestDatabaseSimple_Identity_Change");
        }

        public abstract void Column_Change_Length(SqlEngineVersion version);
        protected static DatabaseDefinitions Column_Change_Length_Dds(SqlEngineVersion version)
        {
            var ddOriginal = new TestDatabaseSimple();
            ddOriginal.SetVersions(version.GetTypeMapper());

            var ddColumnLengthChanged = new TestDatabaseSimple();
            ddColumnLengthChanged.SetVersions(version.GetTypeMapper());
            ddColumnLengthChanged.GetTable("Company")["Name"].Type.Length += 1;

            return new DatabaseDefinitions(ddOriginal, ddColumnLengthChanged, "TestDatabaseSimple", "TestDatabaseSimple_Column_Change_Length");
        }

        public abstract void Column_Change2_Length(SqlEngineVersion version);
        protected static DatabaseDefinitions Column_Change2_Length_Dds(SqlEngineVersion version)
        {
            var ddOriginal = new TestDatabaseSimple();
            ddOriginal.SetVersions(version.GetTypeMapper());
            ddOriginal.GetTable("Company").AddNVarChar("Name2", 100);

            var ddColumnLengthsChanged = new TestDatabaseSimple();
            ddColumnLengthsChanged.SetVersions(version.GetTypeMapper());
            ddColumnLengthsChanged.GetTable("Company").AddNVarChar("Name2", 100);
            ddColumnLengthsChanged.GetTable("Company")["Name"].Type.Length += 1;
            ddColumnLengthsChanged.GetTable("Company")["Name2"].Type.Length += 1;

            return new DatabaseDefinitions(ddOriginal, ddColumnLengthsChanged, "TestDatabaseSimple", "TestDatabaseSimple_Column_Change2_Length");
        }

        public abstract void Column_Change_Nullable(SqlEngineVersion version);
        protected static DatabaseDefinitions Column_Change_Nullable_Dds(SqlEngineVersion version)
        {
            var ddOriginal = new TestDatabaseSimple();
            ddOriginal.SetVersions(version.GetTypeMapper());

            var ddNullableChanged = new TestDatabaseSimple();
            ddNullableChanged.SetVersions(version.GetTypeMapper());
            ddNullableChanged.GetTable("Company")["Name"].Type.IsNullable = !ddOriginal.GetTable("Company")["Name"].Type.IsNullable;

            return new DatabaseDefinitions(ddOriginal, ddNullableChanged, "TestDatabaseSimple", "TestDatabaseSimple_Column_Change_Nullable");
        }

        public abstract void Column_Change_DbType(SqlEngineVersion version);
        protected static DatabaseDefinitions Column_Change_DbType_Dds(SqlEngineVersion version)
        {
            var ddOriginal = new TestDatabaseSimple();
            ddOriginal.SetVersions(version.GetTypeMapper());

            var ddDbTypeChanged = new TestDatabaseSimple();
            ddDbTypeChanged.SetVersions(version.GetTypeMapper());

            if (version == MsSqlVersion.MsSql2016)
                ddDbTypeChanged.GetTable("Company")["Name"].Type.SqlTypeInfo = MsSqlType2016.NChar;
            else if (version == OracleVersion.Oracle12c)
                ddDbTypeChanged.GetTable("Company")["Name"].Type.SqlTypeInfo = OracleType12c.NChar;

            return new DatabaseDefinitions(ddOriginal, ddDbTypeChanged, "TestDatabaseSimple", "TestDatabaseSimple_Column_Change_DbType");
        }

        public abstract void Column_Change_DbTypeAndLengthAndIsNullable(SqlEngineVersion version);
        protected static DatabaseDefinitions Column_Change_DbTypeAndLengthAndIsNullable_Dds(SqlEngineVersion version)
        {
            var ddOriginal = new TestDatabaseSimple();
            ddOriginal.SetVersions(version.GetTypeMapper());

            var ddddDbTypeAndLengthChanged = new TestDatabaseSimple();
            ddddDbTypeAndLengthChanged.SetVersions(version.GetTypeMapper());

            if (version == MsSqlVersion.MsSql2016)
                ddddDbTypeAndLengthChanged.GetTable("Company")["Name"].Type.SqlTypeInfo = MsSqlType2016.NChar;
            else if (version == OracleVersion.Oracle12c)
                ddddDbTypeAndLengthChanged.GetTable("Company")["Name"].Type.SqlTypeInfo = OracleType12c.NChar;

            ddddDbTypeAndLengthChanged.GetTable("Company")["Name"].Type.Length += 1;
            ddddDbTypeAndLengthChanged.GetTable("Company")["Name"].Type.IsNullable = !ddddDbTypeAndLengthChanged.GetTable("Company")["Name"].Type.IsNullable;

            return new DatabaseDefinitions(ddOriginal, ddddDbTypeAndLengthChanged, "TestDatabaseSimple", "TestDatabaseSimple_Column_Change_DbTypeAndLengthAndIsNullable");
        }

        public abstract void Index_Add(SqlEngineVersion version);
        protected static DatabaseDefinitions Index_Add_Dds(SqlEngineVersion version)
        {
            var ddOriginal = new TestDatabaseSimple();
            ddOriginal.SetVersions(version.GetTypeMapper());

            var ddWithNewIndex = new TestDatabaseIndex();
            ddWithNewIndex.SetVersions(version.GetTypeMapper());

            return new DatabaseDefinitions(ddOriginal, ddWithNewIndex, "TestDatabaseSimple", "TestDatabaseIndex_Index_Add");
        }

        public abstract void Index_Change(SqlEngineVersion version);
        protected static DatabaseDefinitions Index_Change_Dds(SqlEngineVersion version)
        {
            var ddOriginal = new TestDatabaseIndexMultiColumn();
            ddOriginal.SetVersions(version.GetTypeMapper());
            ddOriginal.GetTable("Company").Properties.Remove(
                ddOriginal.GetTable("Company").Properties.OfType<Index>().First()
            );
            ddOriginal.GetTable("Company").AddIndexWithName(false, "IX_Company_Name", "Name1");

            var ddWithChangedIndex = new TestDatabaseIndexMultiColumn();
            ddWithChangedIndex.SetVersions(version.GetTypeMapper());
            ddWithChangedIndex.GetTable("Company").Properties.OfType<Index>().First().Name = "IX_Company_Name";

            return new DatabaseDefinitions(ddOriginal, ddWithChangedIndex, "TestDatabaseIndexMultiColumn", "TestDatabaseIndexMultiColum_Index_Change");
        }

        public abstract void UniqueConstraint_Change(SqlEngineVersion version);

        protected static DatabaseDefinitions UniqueConstraint_Change_Dds(SqlEngineVersion version)
        {
            var ddOriginal = new TestDatabaseUniqueConstraint();
            ddOriginal.SetVersions(version.GetTypeMapper());

            ddOriginal.GetTable("Company").AddNVarChar("Name2", 100);

            var ddNew = new TestDatabaseUniqueConstraint2();
            ddNew.SetVersions(version.GetTypeMapper());

            return new DatabaseDefinitions(ddOriginal, ddNew, "TestDatabaseUniqueConstraint_Column_Add", "TestDatabaseUniqueConstraint2");
        }

        public abstract void UniqueConstraint_Change_NewColumn(SqlEngineVersion version);

        protected static DatabaseDefinitions UniqueConstraint_Change_NewColumn_Dds(SqlEngineVersion version)
        {
            var ddOriginal = new TestDatabaseUniqueConstraint();
            ddOriginal.SetVersions(version.GetTypeMapper());

            var ddNew = new TestDatabaseUniqueConstraint2();
            ddNew.SetVersions(version.GetTypeMapper());

            return new DatabaseDefinitions(ddOriginal, ddNew, "TestDatabaseUniqueConstraint", "TestDatabaseUniqueConstraint2");
        }

        public abstract void UniqueConstraint_Change_NewColumn_UcName(SqlEngineVersion version);

        protected static DatabaseDefinitions UniqueConstraint_Change_NewColumn_UcName_Dds(SqlEngineVersion version)
        {
            var ddOriginal = new TestDatabaseUniqueConstraint();
            ddOriginal.SetVersions(version.GetTypeMapper());
            ddOriginal.GetTable("Company").Properties.OfType<UniqueConstraint>().First().Name = "UC_1";

            var ddUcChanged = new TestDatabaseUniqueConstraint2();
            ddUcChanged.SetVersions(version.GetTypeMapper());
            ddUcChanged.GetTable("Company").Properties.OfType<UniqueConstraint>().First().Name = "UC_1";

            return new DatabaseDefinitions(ddOriginal, ddUcChanged, "TestDatabaseUniqueConstraint", "TestDatabaseUniqueConstraint2_Change_NewColumn_UcName_Dds");
        }
    }
}