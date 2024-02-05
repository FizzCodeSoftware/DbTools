using System.Collections.Generic;
using FizzCode.DbTools.DataDefinition;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition.Base.Interfaces;
using FizzCode.DbTools.DataDefinition.Factory;
using FizzCode.DbTools.SqlExecuter;
using FizzCode.DbTools.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// Ensure no in-assembly parallel execution of tests (“IAP”) is happening
[assembly: Parallelize(Workers = 1, Scope = ExecutionScope.ClassLevel)]
namespace FizzCode.DbTools.DataDefinitionReader.Tests;
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
        SqlExecuterTestAdapter.Initialize(version.UniqueName, dd);
        TestHelper.CheckFeature(version, "ReadDdl");

        var databaseCreator = new DatabaseCreator(dd, SqlExecuterTestAdapter.GetExecuter(version.UniqueName));

        databaseCreator.ReCreateDatabase(true);
    }

    [AssemblyCleanup]
    public static void Cleanup()
    {
        SqlExecuterTestAdapter.Cleanup();
    }

    protected IDatabaseDefinition ReadDd(SqlEngineVersion version, IEnumerable<string> schemaNames)
    {
        var schemaNamesToRead = SchemaNamesToRead.ToSchemaNames(schemaNames);
        var ddlReader = _dataDefinitionReaderFactory.CreateDataDefinitionReader(SqlExecuterTestAdapter.ConnectionStrings[version.UniqueName], schemaNamesToRead);

        var dd = new DatabaseDefinition(new TestFactoryContainer(), version, GenericVersion.Generic1);
        var db = ddlReader.GetDatabaseDefinition(dd);

        return db;
    }
}