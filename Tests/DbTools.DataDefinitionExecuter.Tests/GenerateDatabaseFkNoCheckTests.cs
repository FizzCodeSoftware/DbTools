#pragma warning disable CA1034 // Nested types should not be visible
namespace FizzCode.DbTools.DataDefinition.SqlExecuter.Tests
{
    using System;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Generic1;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

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

    public class TestDatabaseFkNoCheckTest : TestDatabaseDeclaration
    {
        public SqlTable Primary { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddNVarChar("Name", 100);
        });

        public SqlTable Foreign { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddInt32("PrimaryId").SetForeignKeyTo(nameof(Primary), new SqlEngineVersionSpecificProperty(MsSqlVersion.MsSql2016, "Nocheck", "true"));
        });
    }

    [TestClass]
    public class GenerateDatabaseFkNoCheckTests : GenerateDatabaseTestsBase
    {
        [TestMethod]
        [LatestSqlVersions]
        public void GenerateTestDatabaseFkNoCheckTest(SqlEngineVersion version)
        {
            GenerateDatabase(new TestDatabaseFkNoCheckTest(), version,
                () => {
                    if (version == MsSqlVersion.MsSql2016)
                    {
                        _sqlExecuterTestAdapter.ExecuteNonQuery(version.ToString(), "INSERT INTO [Primary] (Name) Values ('First')");
                        _sqlExecuterTestAdapter.ExecuteNonQuery(version.ToString(), "INSERT INTO [Foreign] (PrimaryId) Values (1)");
                        _sqlExecuterTestAdapter.ExecuteNonQuery(version.ToString(), "INSERT INTO [Foreign] (PrimaryId) Values (-1)");
                    }
                }
            );
        }
    }
}