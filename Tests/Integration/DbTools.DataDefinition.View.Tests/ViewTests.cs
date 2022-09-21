namespace FizzCode.DbTools.DataDefinition.View.Tests
{
    using FizzCode.DbTools.DataDeclaration;
    using FizzCode.DbTools.DataDefinition.Base;
    using FizzCode.DbTools.DataDefinition.Factory;
    using FizzCode.DbTools.DataDefinition.Generic1;
    using FizzCode.DbTools.QueryBuilder;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ViewTests : ViewTestsBase
    {
        [TestMethod]
        [LatestSqlVersions]
        public void ViewSimple(SqlEngineVersion version)
        {
            Init(version, new TestDatabaseSimpleWithView());
            var result = SqlExecuterTestAdapter.GetExecuter(version.UniqueName).ExecuteQuery("SELECT * FROM CompanyView");

            // TODO test view
            // - is it created
            // - is it runnable
            // - is it returning the expected result

        }
    }

    public class TestDatabaseSimpleWithView : DatabaseDeclaration
    {
        /*protected TestDatabaseSimpleWithView()
            : base(new QueryBuilder(), MsSqlVersion.MsSql2016.GetTypeMapper(), new[] { OracleVersion.Oracle12c.GetTypeMapper() })
        {
        }*/

        public TestDatabaseSimpleWithView()
            : base(new QueryBuilder(), null, new[] { MsSqlVersion.MsSql2016.GetTypeMapper(), OracleVersion.Oracle12c.GetTypeMapper(), SqLiteVersion.SqLite3.GetTypeMapper() })
        {
        }

        public SqlTable Company { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddNVarChar("Name", 100);
        });

        public SqlView CompanyView => new ViewFromQuery(new Query(Company));
    }
}
