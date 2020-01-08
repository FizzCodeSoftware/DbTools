namespace FizzCode.DbTools.DataDefinitionReader.Tests
{
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionExecuter;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public abstract class DataDefinitionReaderTests
    {
        protected static readonly SqlExecuterTestAdapter _sqlExecuterTestAdapter = new SqlExecuterTestAdapter();

        protected static void Init(SqlDialect sqlDialect, DatabaseDefinition dd)
        {
            _sqlExecuterTestAdapter.Check(sqlDialect);
            _sqlExecuterTestAdapter.Initialize(sqlDialect.ToString(), dd);
            TestHelper.CheckFeature(sqlDialect, "ReadDdl");

            _sqlExecuterTestAdapter.GetContext(sqlDialect).Settings.Options.ShouldUseDefaultSchema = true;
        }

        [AssemblyCleanup]
        public static void Cleanup()
        {
            _sqlExecuterTestAdapter.Cleanup();
        }
    }
}