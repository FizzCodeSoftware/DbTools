namespace FizzCode.DbTools.DataDefinitionExecuter
{
    using System;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Text.RegularExpressions;
    using FizzCode.DbTools.DataDefinitionGenerator;

    public class MsSqlExecuter : SqlExecuter
    {
        public MsSqlExecuter(ConnectionStringSettings connectionStringSettings, ISqlGenerator sqlGenerator)
            : base(connectionStringSettings, sqlGenerator)
        {
        }

        public override void CreateDatabase(bool shouldSkipIfExists)
        {
            var builder = new SqlConnectionStringBuilder(ConnectionString);
            var sql = Generator.CreateDatabase(builder.InitialCatalog, shouldSkipIfExists);
            ExecuteNonQueryMaster(sql);
        }

        public override void DropDatabase()
        {
            var builder = new SqlConnectionStringBuilder(ConnectionString);
            var sql = Generator.DropDatabase(builder.InitialCatalog);
            ExecuteNonQueryMaster(sql);
        }

        public override void DropDatabaseIfExists()
        {
            var builder = new SqlConnectionStringBuilder(ConnectionString);
            var sql = Generator.DropDatabaseIfExists(builder.InitialCatalog);
            ExecuteNonQueryMaster(sql);
        }

        public SqlConnection OpenConnection()
        {
            var connection = new SqlConnection(ConnectionString);
            connection.Open();

            return connection;
        }

        public SqlCommand PrepareSqlCommand(SqlStatementWithParameters sqlStatementWithParameters)
        {
            var command = new SqlCommand(sqlStatementWithParameters.Statement);

            foreach (var parameter in sqlStatementWithParameters.Parameters)
                command.Parameters.AddWithValue(parameter.Key, parameter.Value);

            return command;
        }

        public override void ExecuteNonQuery(SqlStatementWithParameters sqlStatementWithParameters)
        {
            var connection = OpenConnection();
            try
            {
                var command = PrepareSqlCommand(sqlStatementWithParameters);
                command.Connection = connection;
                command.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                var newEx = new Exception($"Sql fails:\r\n{sqlStatementWithParameters.Statement}\r\n{ex.Message}", ex);
                throw newEx;
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public override Reader ExecuteQuery(SqlStatementWithParameters sqlStatementWithParameters)
        {
            var connection = OpenConnection();
            try
            {
                var reader = new Reader();

                var command = PrepareSqlCommand(sqlStatementWithParameters);
                command.Connection = connection;

                using (var sqlReader = command.ExecuteReader())
                {
                    while (sqlReader.Read())
                    {
                        var row = new Row();
                        for (var i = 0; i < sqlReader.FieldCount; i++)
                        {
                            row.Add(sqlReader.GetName(i), sqlReader[i]);
                        }

                        reader.Rows.Add(row);
                    }
                }

                return reader;
            }
            catch (SqlException ex)
            {
                var newEx = new Exception($"Sql fails:\r\n{sqlStatementWithParameters.Statement}\r\n{ex.Message}", ex);
                throw newEx;
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        protected override void ExecuteNonQueryMaster(SqlStatementWithParameters sqlStatementWithParameters)
        {
            SqlConnection.ClearAllPools(); // force closing connections to normal database to be able to exetute DDLs.

            var connection = OpenConnectionMaster();
            try
            {
                var command = PrepareSqlCommand(sqlStatementWithParameters);
                command.Connection = connection;
                command.ExecuteNonQuery();
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public override object ExecuteScalar(SqlStatementWithParameters sqlStatementWithParameters)
        {
            object result;

            var connection = OpenConnection();
            try
            {
                var command = PrepareSqlCommand(sqlStatementWithParameters);
                command.Connection = connection;
                result = command.ExecuteScalar();
            }
            catch (SqlException ex)
            {
                var newEx = new Exception($"Sql fails:\r\n{sqlStatementWithParameters.Statement}\r\n{ex.Message}", ex);
                throw newEx;
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }

            return result;
        }

        public SqlConnection OpenConnectionMaster()
        {
            var connection = new SqlConnection(ChangeInitialCatalog(ConnectionString, string.Empty));
            connection.Open();

            return connection;
        }

        // TODO delete?
        protected override string ChangeInitialCatalog(string connectionString)
        {
            return "";
            // return ChangeInitialCatalog(connectionString, InitialCatalog);
        }

        private string ChangeInitialCatalog(string connectionString, string newInitialCatalog)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            if (newInitialCatalog != null)
                builder.InitialCatalog = newInitialCatalog;

            return builder.ConnectionString;
        }
    }
}