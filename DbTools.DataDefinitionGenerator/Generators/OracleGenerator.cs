namespace FizzCode.DbTools.DataDefinitionGenerator
{
    using System.Linq;
    using FizzCode.DbTools.DataDefinition;

    public class OracleGenerator : MsSqlGenerator
    {
        public override SqlStatementWithParameters CreateDatabase(string databaseName, bool shouldSkipIfExists)
        {
            return shouldSkipIfExists
                ? IfExists(new SqlStatementWithParameters($"SELECT INSTANCE_NAME, STATUS, DATABASE_STATUS FROM V$INSTANCE WHERE INSTANCE_NAME = @DatabaseName)", databaseName), "", $"CREATE DATABASE { GuardKeywords(databaseName)}\r\nEND IF")
                : $"CREATE DATABASE {GuardKeywords(databaseName)}";
        }

        public override SqlStatementWithParameters DropDatabaseIfExists(string databaseName)
        {
            return DropDatabase(databaseName) + ";";
            return IfExists(new SqlStatementWithParameters($"SELECT INSTANCE_NAME, STATUS, DATABASE_STATUS FROM V$INSTANCE WHERE INSTANCE_NAME = @DatabaseName)", databaseName), DropDatabase(databaseName), "");
        }

        public new string DropDatabase(string databaseName)
        {
            return $"DROP DATABASE {databaseName}";
        }

        public SqlStatementWithParameters IfExists(SqlStatementWithParameters ifExistsCondition, SqlStatementWithParameters ifExists, SqlStatementWithParameters ifNotExists)
        {
            var sql = string.Format(SqlIfExists, ifExistsCondition.Statement, ifExists.Statement, ifNotExists.Statement);
            var unionParameters = ifExistsCondition.Parameters.Union(ifExists.Parameters.Union(ifNotExists.Parameters)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            var sqls = new SqlStatementWithParameters(sql, unionParameters);

            return sqls;
        }

        private const string SqlIfExists = @"
DECLARE
    l_exst number(1);
BEGIN
    SELECT CASE 
        WHEN EXISTS({0})
        THEN 1
        ELSE 0
    END INTO l_exst
    FROM dual;

    IF l_exst = 1 
    THEN
        {1}
    ELSE
        {2}
    END IF;
END;";
    }
}
