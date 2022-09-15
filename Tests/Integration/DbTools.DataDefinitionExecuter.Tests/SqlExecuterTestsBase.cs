// Ensure no in-assembly parallel execution of tests (“IAP”) is happening
[assembly: Microsoft.VisualStudio.TestTools.UnitTesting.Parallelize(Workers = 1, Scope = Microsoft.VisualStudio.TestTools.UnitTesting.ExecutionScope.ClassLevel)]

namespace FizzCode.DbTools.DataDefinition.SqlExecuter.Tests
{
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public abstract class SqlExecuterTestsBase
    {
        protected static SqlExecuterTestAdapter SqlExecuterTestAdapter { get; } = new();

        [AssemblyCleanup]
        public static void Cleanup()
        {
            SqlExecuterTestAdapter.Cleanup();
        }
    }
}