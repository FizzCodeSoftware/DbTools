namespace FizzCode.DbTools.DataDefinitionExecuter
{
    using System;
    using System.Configuration;
    using System.Data.Common;
    using System.Data.SqlClient;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionGenerator;

    public class MsSqlExecuter : SqlExecuter
    {
        protected override SqlDialect SqlDialect => SqlDialect.MsSql;

        public MsSqlExecuter(ConnectionStringSettings connectionStringSettings, ISqlGenerator sqlGenerator)
            : base(connectionStringSettings, sqlGenerator)
        {
        }

        public override void CreateDatabase(bool shouldSkipIfExists)
        {
            var builder = GetConnectionStringBuilder();
            builder.ConnectionString = ConnectionString;
            var sql = Generator.CreateDatabase(GetDatabase(builder), shouldSkipIfExists);
            ExecuteNonQueryMaster(sql);
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

        public override DbConnection OpenConnectionMaster()
        {
            var connection = new SqlConnection(ChangeInitialCatalog(ConnectionString, string.Empty));
            connection.Open();

            return connection;
        }

        private string ChangeInitialCatalog(string connectionString, string newInitialCatalog)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            if (newInitialCatalog != null)
                builder.InitialCatalog = newInitialCatalog;

            return builder.ConnectionString;
        }

        public override string GetDatabase(DbConnectionStringBuilder builder)
        {
            return builder.ValueOfKey<string>("Initial Catalog");
        }
    }
}