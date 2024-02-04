using FizzCode.DbTools.SqlExecuter;
using FizzCode.DbTools.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// Ensure no in-assembly parallel execution of tests (“IAP”) is happening
[assembly: Parallelize(Workers = 1, Scope = ExecutionScope.ClassLevel)]
namespace FizzCode.DbTools.DataDefinition.Sp.Tests;
[TestClass]
public abstract class SpTestsBase
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

        var databaseCreator = new DatabaseCreator(dd, SqlExecuterTestAdapter.GetExecuter(version.UniqueName));

        databaseCreator.ReCreateDatabase(true);
    }
}