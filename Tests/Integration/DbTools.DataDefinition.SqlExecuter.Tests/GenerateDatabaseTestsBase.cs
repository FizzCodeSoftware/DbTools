using System;
using FizzCode.DbTools.DataDefinition;

#pragma warning disable CA1034 // Nested types should not be visible
namespace FizzCode.DbTools.SqlExecuter.Tests;
public abstract class GenerateDatabaseTestsBase : SqlExecuterTestsBase
{
    protected static void GenerateDatabase(DatabaseDefinition dd, SqlEngineVersion version, Action action = null)
    {
        SqlExecuterTestAdapter.Check(version);
        SqlExecuterTestAdapter.Initialize(version.UniqueName, dd);

        var databaseCreator = new DatabaseCreator(dd, SqlExecuterTestAdapter.GetExecuter(version.UniqueName));

        try
        {
            databaseCreator.ReCreateDatabase(true);
            action?.Invoke();
        }
        finally
        {
            databaseCreator.CleanupDatabase();
        }
    }
}