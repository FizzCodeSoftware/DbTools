using FizzCode.DbTools.DataDefinition;
using FizzCode.DbTools.DataDefinition.Tests;
using FizzCode.DbTools.SqlExecuter;
using FizzCode.DbTools.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// Ensure no in-assembly parallel execution of tests (“IAP”) is happening
[assembly: Parallelize(Workers = 1, Scope = ExecutionScope.ClassLevel)]
namespace FizzCode.DbTools.DataDefinitionExecuterMigrationIntegration.Tests;
[TestClass]
public abstract class DatabaseMigratorTestsBase : ComparerTestsBase
{
    protected static SqlExecuterTestAdapter SqlExecuterTestAdapter { get; } = new();

    [AssemblyCleanup]
    public static void Cleanup()
    {
        SqlExecuterTestAdapter.Cleanup();
    }

    protected static void Init(SqlEngineVersion version, DatabaseDefinition dd)
    {
        SqlExecuterTestAdapter.Check(version);
        SqlExecuterTestAdapter.Initialize(version.UniqueName, dd);
        TestHelper.CheckFeature(version, "ReadDdl");

        var databaseCreator = new DatabaseCreator(dd, SqlExecuterTestAdapter.GetExecuter(version.UniqueName));

        databaseCreator.ReCreateDatabase(true);
    }
}