#pragma warning disable CA1034 // Nested types should not be visible
namespace FizzCode.DbTools.DataDefinition.SqlExecuter.Tests
{
    using System;
    using FizzCode.DbTools.DataDefinition;

    public abstract class GenerateDatabaseTestsBase : DataDefinitionSqlExecuterTests
    {
        protected static void GenerateDatabase(DatabaseDefinition dd, SqlEngineVersion version, Action action = null)
        {
            _sqlExecuterTestAdapter.Check(version);
            _sqlExecuterTestAdapter.Initialize(version.UniqueName, dd);

            var databaseCreator = new DatabaseCreator(dd, _sqlExecuterTestAdapter.GetExecuter(version.UniqueName));

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
}