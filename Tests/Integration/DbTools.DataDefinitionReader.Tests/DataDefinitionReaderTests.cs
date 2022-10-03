// Ensure no in-assembly parallel execution of tests (“IAP”) is happening
[assembly: Microsoft.VisualStudio.TestTools.UnitTesting.Parallelize(Workers = 1, Scope = Microsoft.VisualStudio.TestTools.UnitTesting.ExecutionScope.ClassLevel)]

namespace FizzCode.DbTools.DataDefinitionReader.Tests
{
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Base;
    using FizzCode.DbTools.DataDefinition.Base.Interfaces;
    using FizzCode.DbTools.DataDefinition.Factory;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public abstract class DataDefinitionReaderTests
    {
        protected readonly IDataDefinitionReaderFactory _dataDefinitionReaderFactory;
        public DataDefinitionReaderTests()
        {
            var contextFactory = new TestContextFactory(s => s.Options.ShouldUseDefaultSchema = true);
            _dataDefinitionReaderFactory = new DataDefinitionReaderFactory(contextFactory);
        }

        protected static SqlExecuterTestAdapter SqlExecuterTestAdapter { get; } = new();

        protected static void Init(SqlEngineVersion version, DatabaseDefinition dd)
        {
            SqlExecuterTestAdapter.Check(version);
            if (dd == null)
                SqlExecuterTestAdapter.Initialize(version.UniqueName);
            else
                SqlExecuterTestAdapter.Initialize(version.UniqueName, dd);

            TestHelper.CheckFeature(version, "ReadDdl");
        }

        [AssemblyCleanup]
        public static void Cleanup()
        {
            SqlExecuterTestAdapter.Cleanup();
        }

        protected IDatabaseDefinition ReadDd(SqlEngineVersion version, SchemaNamesToRead schemaNames)
        {
            var ddlReader = _dataDefinitionReaderFactory.CreateDataDefinitionReader(SqlExecuterTestAdapter.ConnectionStrings[version.UniqueName], schemaNames);
            var db = ddlReader.GetDatabaseDefinition();

            return db;
        }
    }
}