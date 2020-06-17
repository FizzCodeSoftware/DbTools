namespace FizzCode.DbTools.DataDefinition.SqlExecuterMigrationIntegration.Tests
{
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition.SqlExecuter;
    using FizzCode.DbTools.DataDefinition.Tests;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public abstract class DatabaseMigratorTestsBase : ComparerTestsBase
    {
        protected static SqlExecuterTestAdapter SqlExecuterTestAdapter { get; } = new SqlExecuterTestAdapter();

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

            SqlExecuterTestAdapter.GetContext(version).Settings.Options.ShouldUseDefaultSchema = true;

            var databaseCreator = new DatabaseCreator(dd, SqlExecuterTestAdapter.GetExecuter(version.UniqueName));

            databaseCreator.ReCreateDatabase(true);
        }
    }
}