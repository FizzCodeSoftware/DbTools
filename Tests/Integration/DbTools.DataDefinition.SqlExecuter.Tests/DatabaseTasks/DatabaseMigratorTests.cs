using System.Linq;
using FizzCode.DbTools.DataDeclaration;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition.Base.Migration;
using FizzCode.DbTools.DataDefinition.Factory;
using FizzCode.DbTools.DataDefinition.Generic1;
using FizzCode.DbTools.DataDefinition.Tests;
using FizzCode.DbTools.Factory.Interfaces;
using FizzCode.DbTools.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FizzCode.DbTools.SqlExecuter.Tests;
[TestClass]
public class DatabaseMigratorTests : SqlExecuterTestsBase
{
    private readonly ISqlMigrationGeneratorFactory _sqlMigrationGeneratorFactory;

    public DatabaseMigratorTests()
    {
        var contextFactory = new ContextFactory(TestHelper.CreateLogger());
        _sqlMigrationGeneratorFactory = new SqlMigrationGeneratorFactory(contextFactory);
    }

    [TestMethod]
    [LatestSqlVersions]
    public void NewTableTest(SqlEngineVersion version)
    {
        SqlExecuterTestAdapter.Check(version);
        var dd = new TestDatabaseSimple();
        SqlExecuterTestAdapter.InitializeAndCreate(version.UniqueName, dd);

        var migrationGenerator = _sqlMigrationGeneratorFactory.CreateMigrationGenerator(version);

        var executer = SqlExecuterTestAdapter.GetExecuter(version.UniqueName);

        var databaseMigrator = new DatabaseMigrator(executer, migrationGenerator);
        var tableNew = new TableNew
        {
            SchemaAndTableName = "NewTableToMigrate"
        };
        tableNew.AddInt32("Id", false).SetPK().SetIdentity();

        new PrimaryKeyNamingDefaultStrategy().SetPrimaryKeyName(tableNew.Properties.OfType<PrimaryKey>().First());

        ((SqlTable)tableNew).AddNVarChar("Name", 100);

        dd.AddTable(tableNew);

        databaseMigrator.NewTable(tableNew);
    }
}