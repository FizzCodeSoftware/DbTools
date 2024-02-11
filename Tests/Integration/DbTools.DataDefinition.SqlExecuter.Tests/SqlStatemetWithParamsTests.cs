using System.Linq;
using FizzCode.DbTools.Common;
using FizzCode.DbTools.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FizzCode.DbTools.SqlExecuter.Tests;
[TestClass]
public class SqlStatemetWithParamsTests : SqlExecuterTestsBase
{
    [TestMethod]
    [LatestSqlVersions]
    public void ParamSimple(SqlEngineVersion version)
    {
        Init(version);

        var executer = SqlExecuterTestAdapter.GetExecuter(version.UniqueName);

        var sql = "SELECT 1 "
            + (version is MsSqlVersion
                || version is SqLiteVersion ? "" : "FROM dual ")
            + "WHERE @var = 1";
        
        var sqlStatement = new SqlStatementWithParameters(sql, 1);
        var result = executer.ExecuteQuery(sqlStatement);

        Assert.AreEqual(1, result.Count);
        var resultString = result[0].Values.First().ToString();
        Assert.IsNotNull(resultString);
        var intResult = int.Parse(resultString);
        Assert.AreEqual(1, intResult);

    }

    [TestMethod]
    [LatestSqlVersions]
    public void ParamNull(SqlEngineVersion version)
    {
        Init(version);

        var executer = SqlExecuterTestAdapter.GetExecuter(version.UniqueName);

        var sql = "SELECT 1  "
            + (version is MsSqlVersion
                || version is SqLiteVersion ? "" : "FROM dual ")
            + "WHERE @var = 1";

        var sqlStatement = new SqlStatementWithParameters(sql, [null]);
        var result = executer.ExecuteQuery(sqlStatement);

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    [LatestSqlVersions]
    public void ParamIsNull(SqlEngineVersion version)
    {
        Init(version);

        var executer = SqlExecuterTestAdapter.GetExecuter(version.UniqueName);

        var sql = "SELECT 1  "
            + (version is MsSqlVersion
                || version is SqLiteVersion ? "" : "FROM dual ")
            + "WHERE @var IS NULL";

        var sqlStatement = new SqlStatementWithParameters(sql, [null]);
        var result = executer.ExecuteQuery(sqlStatement);

        Assert.AreEqual(1, result.Count);
        var resultString = result[0].Values.First().ToString();
        Assert.IsNotNull(resultString);
        var intResult = int.Parse(resultString);
        Assert.AreEqual(1, intResult);
    }

}
