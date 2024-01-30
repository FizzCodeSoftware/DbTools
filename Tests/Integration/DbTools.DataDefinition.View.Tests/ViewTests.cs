namespace FizzCode.DbTools.DataDefinition.View.Tests
{
    using FizzCode.DbTools.DataDefinition.Tests;
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

            SqlExecuterTestAdapter.GetExecuter(version.UniqueName).ExecuteNonQuery("INSERT INTO Company (Name) VALUES ('FirstCompanyName')");
            SqlExecuterTestAdapter.GetExecuter(version.UniqueName).ExecuteNonQuery("INSERT INTO Company (Name) VALUES ('SecondCompanyName')");

            var result = SqlExecuterTestAdapter.GetExecuter(version.UniqueName).ExecuteQuery("SELECT * FROM \"CompanyView\"");

            // TODO test view
            // - is it created
            // - is it runnable
            // - is it returning the expected result

        }
    }
}
