using System.Linq;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition.Base.Migration;
using FizzCode.DbTools.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FizzCode.DbTools.DataDefinitionExecuterMigrationIntegration.Tests;

public partial class DatabaseMigratorTests : DatabaseMigratorTestsBase
{
    public override void DefaultValue_Add(SqlEngineVersion version)
    {
        var dds = DefaultValue_Add_Dds(version);
        var databaseMigrator = ProcessAndGetMigrator(version, dds, out var changes);
        var first = Assert.That.CheckAndReturnInstanceOfType<ColumnChange>(changes[0]);

        databaseMigrator.ChangeColumns(first);
    }

    [TestMethod]
    [LatestSqlVersions]
    public override void DefaultValue_Remove(SqlEngineVersion version)
    {
        var dds = DefaultValue_Remove_Dds(version);

        var databaseMigrator = ProcessAndGetMigrator(version, dds, out var changes);
        var first = Assert.That.CheckAndReturnInstanceOfType<ColumnChange>(changes[0]);

        databaseMigrator.ChangeColumns(first);

        var ddInDb = ReadDd(version);

        if (version is OracleVersion)
        {
            var defaultValue = ddInDb.GetTable("Company").Columns["Name"].Properties.OfType<DefaultValue>().FirstOrDefault();
            Assert.IsNotNull(defaultValue);
            Assert.AreEqual("NULL", defaultValue.Value);
        }
        else
        { 
            Assert.That.IsFalse(
                ddInDb.GetTable("Company").Columns["Name"].HasProperty<DefaultValue>()
                );
        }
    }
}