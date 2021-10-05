namespace FizzCode.DbTools.DataDefinition.SqlExecuter.Tests
{
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public abstract class DataDefinitionSqlExecuterTests
    {
        protected static readonly SqlExecuterTestAdapter _sqlExecuterTestAdapter = new();

        [AssemblyCleanup]
        public static void Cleanup()
        {
            _sqlExecuterTestAdapter.Cleanup();
        }
    }
}