namespace FizzCode.DbTools.DataDefinition.SqlExecuterMigrationIntegration.Tests
{
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public abstract class DataDefinitionExecuterMigrationIntegrationTests
    {
        protected static SqlExecuterTestAdapter SqlExecuterTestAdapter { get; } = new SqlExecuterTestAdapter();

        [AssemblyCleanup]
        public static void Cleanup()
        {
            SqlExecuterTestAdapter.Cleanup();
        }
    }
}