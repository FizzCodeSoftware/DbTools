using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FizzCode.DbTools.DataDeclaration;
using FizzCode.DbTools.DataDefinition;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition.Base.Interfaces;
using FizzCode.DbTools.DataDefinition.Base.Migration;
using FizzCode.DbTools.DataDefinition.Factory;
using FizzCode.DbTools.DataDefinition.Generic;
using FizzCode.DbTools.DataDefinition.Tests;
using FizzCode.DbTools.DataDefinitionReader;
using FizzCode.DbTools.Factory.Interfaces;
using FizzCode.DbTools.SqlExecuter;
using FizzCode.DbTools.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FizzCode.DbTools.DataDefinitionExecuterMigrationIntegration.Tests;
[TestClass]
public class DatabaseMigratorTests : DatabaseMigratorTestsBase
{
    private readonly IContextFactory _contextFactory;
    private readonly ISqlMigrationGeneratorFactory _sqlMigrationGeneratorFactory;
    private readonly IDataDefinitionReaderFactory _dataDefinitionReaderFactory;

    public DatabaseMigratorTests()
    {
        _contextFactory = new TestContextFactory(s => s.Options.ShouldUseDefaultSchema = true);
        _sqlMigrationGeneratorFactory = new SqlMigrationGeneratorFactory(_contextFactory);
        _dataDefinitionReaderFactory = new DataDefinitionReaderFactory(_contextFactory);
    }

    [TestMethod]
    [LatestSqlVersions]
    public override void Fk_Add(SqlEngineVersion version)
    {
        var dds = Fk_Add_Dds(version);

        var databaseMigrator = ProcessAndGetMigrator(version, dds, out var changes);

        var foreignKeyNew = (ForeignKeyNew)changes[0];

        databaseMigrator.NewForeignKey(foreignKeyNew);
    }

    [TestMethod]
    [LatestSqlVersions]
    public override void Fk_Remove(SqlEngineVersion version)
    {
        var dds = Fk_Remove_Dds(version);
        /* var databaseMigrator = */
        ProcessAndGetMigrator(version, dds, out var changes);

        var _ = changes[0] as ForeignKeyDelete;

        // TODO remove FK
        // databaseMigrator.
    }

    [TestMethod]
    [LatestSqlVersions]
    public override void Fk_Change_Composite_NameChange(SqlEngineVersion version)
    {
        var dds = Fk_Change_Composite_NameChange_Dds(version);
        /* var databaseMigrator = */
        ProcessAndGetMigrator(version, dds, out var changes);

        var _ = changes[0] as ForeignKeyChange;

        // TODO change FK
        // databaseMigrator.
    }

    [TestMethod]
    [LatestSqlVersions]
    public override void Fk_Change_Composite_NoNameChange(SqlEngineVersion version)
    {
        var dds = Fk_Change_Composite_NoNameChange_Dds(version);
        /* var databaseMigrator = */
        ProcessAndGetMigrator(version, dds, out var changes);

        var _ = changes[0] as ForeignKeyChange;

        // TODO change FK
        // databaseMigrator.
    }

    [TestMethod]
    [LatestSqlVersions]
    public override void Fk_Change(SqlEngineVersion version)
    {
        var dds = Fk_Change_Dds(version);
        /* var databaseMigrator = */
        ProcessAndGetMigrator(version, dds, out var changes);

        var _ = changes[0] as ForeignKeyChange;

        // TODO change FK
        // databaseMigrator.
    }

    [TestMethod]
    public void FkCheckNoCheckTest()
    {
        var version = MsSqlVersion.MsSql2016;

        var dd = new TestDatabaseFk();
        Init(version, dd);

        var ddlReader = _dataDefinitionReaderFactory.CreateDataDefinitionReader(
            SqlExecuterTestAdapter.ConnectionStrings[version.UniqueName]
            , SchemaNamesToRead.ToSchemaNames(dd.GetSchemaNames()));
        var ddInDatabase = ddlReader.GetDatabaseDefinition();

        var fk = dd.GetTable("Foreign").Properties.OfType<ForeignKey>().First();

        Assert.AreEqual("true", fk.SqlEngineVersionSpecificProperties[version, "Nocheck"]);

        fk.SqlEngineVersionSpecificProperties[version, "Nocheck"] = "false";

        var comparer = new Comparer();
        var changes = comparer.Compare(ddInDatabase, dd);

        _ = changes[0] as ForeignKeyChange;

        _ = new DatabaseMigrator(SqlExecuterTestAdapter.GetExecuter(version.UniqueName), _sqlMigrationGeneratorFactory.CreateMigrationGenerator(version));

        // TODO change FK
        // databaseMigrator.
    }

    [TestMethod]
    [LatestSqlVersions]
    public override void Table_Add(SqlEngineVersion version)
    {
        var dds = Table_Add_Dds(version);
        var databaseMigrator = ProcessAndGetMigrator(version, dds, out var changes);
        var first = changes[0] as TableNew;
        databaseMigrator.NewTable(first);
    }

    [TestMethod]
    [LatestSqlVersions]
    public override void Table_Remove(SqlEngineVersion version)
    {
        var dds = Table_Remove_Dds(version);
        var databaseMigrator = ProcessAndGetMigrator(version, dds, out var changes);
        var first = changes[0] as TableDelete;
        databaseMigrator.DeleteTable(first);
    }

    [TestMethod]
    [LatestSqlVersions]
    public override void Pk_Add(SqlEngineVersion version)
    {
        var dds = Pk_Add_Dds(version);

        var databaseMigrator = ProcessAndGetMigrator(version, dds, out var changes);

        var primaryKeyNew = changes[0] as PrimaryKeyNew;
        databaseMigrator.NewPrimaryKey(primaryKeyNew);

        var ddlReader = _dataDefinitionReaderFactory.CreateDataDefinitionReader(SqlExecuterTestAdapter.ConnectionStrings[version.UniqueName], SchemaNamesToRead.ToSchemaNames(dds.Original.GetSchemaNames()));
        var ddInDatabase = ddlReader.GetDatabaseDefinition();

        var newPk = ddInDatabase.GetTable("Company").Properties.OfType<PrimaryKey>().First();
        Assert.AreEqual(1, newPk.SqlColumns.Count);
        Assert.AreEqual("Id", newPk.SqlColumns[0].SqlColumn.Name);
        Assert.AreEqual("PK_Company", newPk.Name, true, CultureInfo.InvariantCulture);
    }

    [TestMethod]
    [LatestSqlVersions]
    public override void Identity_Change(SqlEngineVersion version)
    {
        var dds = Identity_Change_Dds(version);

        /* var databaseMigrator = */
        ProcessAndGetMigrator(version, dds, out var changes);

        var _ = changes[0] as ColumnChange;
        // databaseMigrator.;
    }

    private DatabaseMigrator ProcessAndGetMigrator(SqlEngineVersion version, DatabaseDefinitions dds, out List<IMigration> changes)
    {
        Init(version, dds.Original);

        var ddlReader = _dataDefinitionReaderFactory.CreateDataDefinitionReader(SqlExecuterTestAdapter.ConnectionStrings[version.UniqueName], SchemaNamesToRead.ToSchemaNames(dds.Original.GetSchemaNames()));
        var ddInDatabase = ddlReader.GetDatabaseDefinition();

        var comparer = new Comparer();
        changes = comparer.Compare(ddInDatabase, dds.New);

        var databaseMigrator = new DatabaseMigrator(SqlExecuterTestAdapter.GetExecuter(version.UniqueName), _sqlMigrationGeneratorFactory.CreateMigrationGenerator(version));

        return databaseMigrator;
    }

    private static void AddTable(DatabaseDefinition dd)
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
    [LatestSqlVersions]
    public override void Column_Remove(SqlEngineVersion version)
    {
        var dds = Column_Remove_Dds(version);
        var databaseMigrator = ProcessAndGetMigrator(version, dds, out var changes);
        var first = changes[0] as ColumnDelete;
        databaseMigrator.DeleteColumns(first);
    }

    [TestMethod]
    [LatestSqlVersions]
    public override void Column_Remove2(SqlEngineVersion version)
    {
        var dds = Column_Remove2_Dds(version);
        var databaseMigrator = ProcessAndGetMigrator(version, dds, out var changes);
        var columnDeleteArray = changes.Cast<ColumnDelete>().ToArray();
        databaseMigrator.DeleteColumns(columnDeleteArray);
    }

    // TODO implement Defaultvalue, generate change order (default before CulumNew) in Comparer
    /*
    [TestMethod]
    [LatestSqlVersions]
    public void AddColumnNotNullWithDefaultValueTest(SqlEngineVersion version)
    {
        var dd = new TestDatabaseSimple();
        dd.SetVersions(version.GetTypeMapper());
        Init(version, dd);

        _sqlExecuterTestAdapter.GetExecuter(version.UniqueName).ExecuteNonQuery("INSERT INTO Company (Name) VALUES ('AddColumnNotNullTestValue')");

        var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(
            _sqlExecuterTestAdapter.ConnectionStrings[version.UniqueName]
            , _sqlExecuterTestAdapter.GetContext(version), dd.GetSchemaNames().ToList());
        var ddInDatabase = ddlReader.GetDatabaseDefinition();

        dd.GetTable("Company").AddVarChar("Name2", 100, false).AddDefaultValue("'default'");

        var comparer = new Comparer(_sqlExecuterTestAdapter.GetContext(version));
        var changes = comparer.Compare(ddInDatabase, dd);

        var first = changes[0] as ColumnNew;
        Assert.AreEqual("Name2", first.SqlColumn.Name);

        var databaseMigrator = new DatabaseMigrator(_sqlExecuterTestAdapter.GetExecuter(version.UniqueName), SqlGeneratorFactory.CreateMigrationGenerator(version, _sqlExecuterTestAdapter.GetContext(version)));

        databaseMigrator.CreateColumns(first);
    }*/

    [TestMethod]
    [LatestSqlVersions]
    public override void Column_Add(SqlEngineVersion version)
    {
        var dds = Column_Add_Dds(version);
        var databaseMigrator = ProcessAndGetMigrator(version, dds, out var changes);
        var first = changes[0] as ColumnNew;
        databaseMigrator.CreateColumns(first);
    }

    [TestMethod]
    [LatestSqlVersions]
    public override void Column_Add2(SqlEngineVersion version)
    {
        var dds = Column_Add2_Dds(version);
        var databaseMigrator = ProcessAndGetMigrator(version, dds, out var changes);
        var columnNewArray = changes.Cast<ColumnNew>().ToArray();
        databaseMigrator.CreateColumns(columnNewArray);
    }

    [TestMethod]
    [LatestSqlVersions]
    public override void Column_Change_Length(SqlEngineVersion version)
    {
        TestHelper.CheckFeature(version, "ColumnLength");

        var dds = Column_Change_Length_Dds(version);
        var databaseMigrator = ProcessAndGetMigrator(version, dds, out var changes);
        var first = changes[0] as ColumnChange;
        databaseMigrator.ChangeColumns(first);
    }

    [TestMethod]
    [LatestSqlVersions]
    public override void Column_Change_Nullable(SqlEngineVersion version)
    {
        var dds = Column_Change_Nullable_Dds(version);
        var databaseMigrator = ProcessAndGetMigrator(version, dds, out var changes);
        var first = changes[0] as ColumnChange;
        databaseMigrator.ChangeColumns(first);
    }

    [TestMethod]
    [LatestSqlVersions]
    public override void Column_Change_NotNullableToNullable_WithFk(SqlEngineVersion version)
    {
        var dds = Column_Change_NotNullableToNullable_WithFk_Dds(version);
        var databaseMigrator = ProcessAndGetMigrator(version, dds, out var changes);
        var columnChange = (ColumnChange)changes[0];
        _ = (ForeignKeyChange)changes[1];
        databaseMigrator.ChangeColumns(columnChange);
    }

    [TestMethod]
    [LatestSqlVersions]
    public override void Column_Change2_Length(SqlEngineVersion version)
    {
        TestHelper.CheckFeature(version, "ColumnLength");

        var dds = Column_Change2_Length_Dds(version);
        var databaseMigrator = ProcessAndGetMigrator(version, dds, out var changes);
        var columnChangesArray = changes.Cast<ColumnChange>().ToArray();
        databaseMigrator.ChangeColumns(columnChangesArray);
    }

    [TestMethod]
    [LatestSqlVersions]
    public override void Index_Add(SqlEngineVersion version)
    {
        var dds = Index_Add_Dds(version);

        /* var databaseMigrator = */
        ProcessAndGetMigrator(version, dds, out var changes);

        _ = (IndexNew)changes[0];

        // databaseMigrator.NewIndex(first);
    }

    [TestMethod]
    [LatestSqlVersions]
    public override void Index_Change(SqlEngineVersion version)
    {
        var dds = Index_Change_Dds(version);

        /* var databaseMigrator = */
        ProcessAndGetMigrator(version, dds, out var changes);

        _ = (IndexChange)changes[0];

        // databaseMigrator
    }

    [TestMethod]
    [SqlVersions(nameof(MsSqlVersion.MsSql2016), nameof(OracleVersion.Oracle12c))]
    public override void Column_Change_DbType(SqlEngineVersion version)
    {
        var dds = Column_Change_DbType_Dds(version);
        var databaseMigrator = ProcessAndGetMigrator(version, dds, out var changes);
        var first = changes[0] as ColumnChange;
        databaseMigrator.ChangeColumns(first);
    }

    [TestMethod]
    [SqlVersions(nameof(MsSqlVersion.MsSql2016), nameof(OracleVersion.Oracle12c))]
    public override void Column_Change_DbTypeAndLengthAndIsNullable(SqlEngineVersion version)
    {
        var dds = Column_Change_DbTypeAndLengthAndIsNullable_Dds(version);
        var databaseMigrator = ProcessAndGetMigrator(version, dds, out var changes);
        var first = changes[0] as ColumnChange;
        databaseMigrator.ChangeColumns(first);
    }

    [TestMethod]
    [LatestSqlVersions]
    public override void UniqueConstraint_Change(SqlEngineVersion version)
    {
        var dds = UniqueConstraint_Change_Dds(version);
        /* var databaseMigrator = */
        ProcessAndGetMigrator(version, dds, out var changes);
        _ = (UniqueConstraintDelete)changes[0];
        _ = (UniqueConstraintNew)changes[1];
        // databaseMigrator.
    }

    [TestMethod]
    [LatestSqlVersions]
    public override void UniqueConstraint_Change_NewColumn(SqlEngineVersion version)
    {
        var dds = UniqueConstraint_Change_NewColumn_Dds(version);
        /* var databaseMigrator = */
        ProcessAndGetMigrator(version, dds, out var changes);
        _ = (ColumnNew)changes[0];
        _ = (UniqueConstraintDelete)changes[1];
        _ = (UniqueConstraintNew)changes[2];
        // databaseMigrator.
    }

    [TestMethod]
    [LatestSqlVersions]
    public override void UniqueConstraint_Change_NewColumn_UcName(SqlEngineVersion version)
    {
        var dds = UniqueConstraint_Change_NewColumn_UcName_Dds(version);
        /* var databaseMigrator = */
        ProcessAndGetMigrator(version, dds, out var changes);
        _ = (ColumnNew)changes[0];
        _ = (UniqueConstraintChange)changes[1];
        // var first = changes[0] as ColumnChange;
        // databaseMigrator.
    }
}