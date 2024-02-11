using FizzCode.DbTools.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// Ensure no in-assembly parallel execution of tests (“IAP”) is happening
[assembly: Parallelize(Workers = 1, Scope = ExecutionScope.ClassLevel)]
namespace FizzCode.DbTools.SqlExecuter.Tests;
[TestClass]
public abstract class SqlExecuterTestsBase
{
    protected static SqlExecuterTestAdapter SqlExecuterTestAdapter { get; } = new();

    [AssemblyCleanup]
    public static void Cleanup()
    {
        SqlExecuterTestAdapter.Cleanup();
    }

    protected static void Init(SqlEngineVersion version)
    {
        var dd = new DatabaseEmpty();

        SqlExecuterTestAdapter.Check(version);
        SqlExecuterTestAdapter.Initialize(version.UniqueName, dd);

        var databaseCreator = new DatabaseCreator(dd, SqlExecuterTestAdapter.GetExecuter(version.UniqueName));

        databaseCreator.ReCreateDatabase(true);
    }
}
