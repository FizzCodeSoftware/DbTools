namespace FizzCode.DbTools.DataDefinitionExecuter
{
    using System;
    using System.Data.Common;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionGenerator;

    public class OracleExecuter : SqlExecuter
    {
        protected override SqlDialect SqlDialect => SqlDialect.Oracle;

        public OracleExecuter(ConnectionStringWithProvider connectionStringWithProvider, ISqlGenerator sqlGenerator)
            : base(connectionStringWithProvider, sqlGenerator)
        {
        }

        public override DbConnection OpenConnectionMaster()
        {
            return OpenConnection();
        }

        public override string GetDatabase()
        {
            throw new NotImplementedException("Oracle executer does not handle database name.");
        }

        public override void InitializeDatabase(bool dropIfExists, params DatabaseDefinition[] dd)
        {
            var defaultSchema = Generator.Settings.SqlDialectSpecificSettings.GetAs<string>("DefaultSchema");

            if (dropIfExists && CheckIfUserExists(defaultSchema))
                CleanupDatabase(dd);

            ExecuteNonQuery($"CREATE USER \"{defaultSchema}\" IDENTIFIED BY sa123");
            ExecuteNonQuery($"GRANT CONNECT, DBA TO \"{defaultSchema}\"");
            ExecuteNonQuery($"GRANT CREATE SESSION TO \"{defaultSchema}\"");
            ExecuteNonQuery($"GRANT UNLIMITED TABLESPACE TO \"{defaultSchema}\"");

            /*var builder = GetConnectionStringBuilder();
            builder.ConnectionString = ConnectionString;
            var connect_identifier = "//" + builder.ValueOfKey("DATA SOURCE");
            ExecuteQuery($"CONNECT {defaultSchema}/sa123@{connect_identifier}");*/

            ExecuteNonQuery($"ALTER SESSION SET current_schema = \"{defaultSchema}\"");
        }

        public bool CheckIfUserExists(string userName)
        {
            var result = ExecuteScalar(OracleGenerator.IfExists("dba_users", "username", userName));
            return (decimal)result == 0;
        }

        public override void CleanupDatabase(params DatabaseDefinition[] dds)
        {
            var defaultSchema = Generator.Settings.SqlDialectSpecificSettings.GetAs<string>("DefaultSchema");
            // TODO - DROP ALL Schemas - in current DD

            var currentUser = ExecuteQuery("select user from dual").Rows[0].GetAs<string>("USER");

            ExecuteQuery($"ALTER SESSION SET current_schema = {currentUser}");
            ExecuteQuery($"DROP USER \"{defaultSchema}\" CASCADE");
        }

        public override DbCommand PrepareSqlCommand(SqlStatementWithParameters sqlStatementWithParameters)
        {
            var dbCommand = base.PrepareSqlCommand(sqlStatementWithParameters);
            return OracleDbCommandPreparer.PrepareSqlCommand(dbCommand);
        }

        public override void ExecuteNonQuery(SqlStatementWithParameters sqlStatementWithParameters)
        {
            if (!(sqlStatementWithParameters.Statement.Trim().StartsWith("BEGIN", StringComparison.CurrentCultureIgnoreCase)
                && sqlStatementWithParameters.Statement.Trim().EndsWith("END;", StringComparison.CurrentCultureIgnoreCase)))
            {
                var count = 0;
                foreach (var c in sqlStatementWithParameters.Statement)
                {
                    if (c == ';')
                        count++;

                    if (count > 1)
                        break;
                }

                var sqlStatementTrimEnd = sqlStatementWithParameters.Statement.TrimEnd();

                if (count == 1 && sqlStatementTrimEnd[^1] == ';')
                {
                    sqlStatementWithParameters.Statement = sqlStatementTrimEnd.Remove(sqlStatementTrimEnd.Length - 1);
                }
                else if (count > 1)
                {
                    foreach (var subStatement in sqlStatementWithParameters.Statement.Split(';'))
                    {
                        if (subStatement.Trim().Length > 0)
                            base.ExecuteNonQuery(new SqlStatementWithParameters(subStatement, sqlStatementWithParameters.Parameters));
                    }
                }
                else
                {
                    base.ExecuteNonQuery(sqlStatementWithParameters);
                }
            }
            else
            {
                base.ExecuteNonQuery(sqlStatementWithParameters);
            }
        }

        protected override void ExecuteNonQueryMaster(SqlStatementWithParameters sqlStatementWithParameters)
        {
            ExecuteQuery(sqlStatementWithParameters);
        }
    }
}