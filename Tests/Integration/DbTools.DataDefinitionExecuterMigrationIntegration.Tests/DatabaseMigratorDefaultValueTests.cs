using FizzCode.DbTools.DataDefinition.Base.Migration;
using FizzCode.DbTools.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FizzCode.DbTools.DataDefinitionExecuterMigrationIntegration.Tests;

public partial class DatabaseMigratorTests : DatabaseMigratorTestsBase
{
    [TestMethod]
    [LatestSqlVersions]
    public override void DefaultValue_Remove(SqlEngineVersion version)
    {
        var dds = DefaultValue_Remove_Dds(version);

        var databaseMigrator = ProcessAndGetMigrator(version, dds, out var changes);
        var first = Assert.That.CheckAndReturnInstanceOfType<ColumnChange>(changes[0]);

        databaseMigrator.ChangeColumns(first);
    }
}