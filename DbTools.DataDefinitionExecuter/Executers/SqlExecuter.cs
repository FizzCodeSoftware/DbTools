namespace FizzCode.DbTools.DataDefinitionExecuter
{
    using System;
    using System.Data.Common;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionGenerator;

    public abstract class SqlExecuter : ISqlExecuter
    {
        protected abstract SqlDialect SqlDialect { get; }

        public ConnectionStringWithProvider ConnectionStringWithProvider { get; }
        public ISqlGenerator Generator { get; }

        
        protected SqlExecuter(ConnectionStringWithProvider connectionStringWithProvider, ISqlGenerator sqlGenerator)
        {
            Generator = sqlGenerator;

            ConnectionStringWithProvider = connectionStringWithProvider;
        }

        protected abstract void ExecuteNonQueryMaster(SqlStatementWithParameters sqlStatementWithParameters);

        public DbConnection OpenConnection()
        {
            var dbf = DbProviderFactories.GetFactory(SqlDialectHelper.GetProviderNameFromSqlDialect(SqlDialect));

            var connection = dbf.CreateConnection();
            connection.ConnectionString = ConnectionStringWithProvider.ConnectionString;
            connection.Open();

            return connection;
        }

        public virtual DbConnection OpenConnectionMaster()
        {
            var dbf = DbProviderFactories.GetFactory(SqlDialectHelper.GetProviderNameFromSqlDialect(SqlDialect));

            var connection = dbf.CreateConnection();
            connection.Open();

            return connection;
        }

        public virtual DbCommand PrepareSqlCommand(SqlStatementWithParameters sqlStatementWithParameters)
        {
            var dbf = DbProviderFactories.GetFactory(SqlDialectHelper.GetProviderNameFromSqlDialect(SqlDialect));

            var command = dbf.CreateCommand();
#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
            command.CommandText = sqlStatementWithParameters.Statement;
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities

            foreach (var parameter in sqlStatementWithParameters.Parameters)
            {
                var dbParameter = command.CreateParameter();
                dbParameter.ParameterName = parameter.Key;
                dbParameter.Value = parameter.Value;
                command.Parameters.Add(dbParameter);
            }

            return command;
        }

        public DbConnectionStringBuilder GetConnectionStringBuilder()
        {
            var dbf = DbProviderFactories.GetFactory(SqlDialectHelper.GetProviderNameFromSqlDialect(SqlDialect));
            return dbf.CreateConnectionStringBuilder();
        }

        public abstract string GetDatabase();

        public abstract void InitializeDatabase(bool dropIfExists, params DatabaseDefinition[] dd);

        public abstract void CleanupDatabase(params DatabaseDefinition[] dds);

        public virtual void ExecuteNonQuery(SqlStatementWithParameters sqlStatementWithParameters)
        {
            var connection = OpenConnection();
            var command = PrepareSqlCommand(sqlStatementWithParameters);
            command.Connection = connection;
            command.CommandTimeout = 60 * 60;
            try
            {
                command.ExecuteNonQuery();
            }
            catch (DbException ex)
            {
                var newEx = new Exception($"Sql fails:\r\n{command.CommandText}\r\n{ex.Message}", ex);
                throw newEx;
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public virtual RowSet ExecuteQuery(SqlStatementWithParameters sqlStatementWithParameters)
        {
            var connection = OpenConnection();

            var command = PrepareSqlCommand(sqlStatementWithParameters);
            command.Connection = connection;

            try
            {
                var rowSet = new RowSet();
                using (var sqlReader = command.ExecuteReader())
                {
                    while (sqlReader.Read())
                    {
                        var row = new Row();
                        for (var i = 0; i < sqlReader.FieldCount; i++)
                        {
                            row.Add(sqlReader.GetName(i), sqlReader[i]);
                        }

                        rowSet.Rows.Add(row);
                    }
                }

                return rowSet;
            }
            catch (DbException ex)
            {
                var newEx = new Exception($"Sql fails:\r\n{command.CommandText}\r\n{ex.Message}", ex);
                throw newEx;
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public virtual object ExecuteScalar(SqlStatementWithParameters sqlStatementWithParameters)
        {
            object result;

            var connection = OpenConnection();
            var command = PrepareSqlCommand(sqlStatementWithParameters);
            command.Connection = connection;

            try
            {
                result = command.ExecuteScalar();
            }
            catch (DbException ex)
            {
                var newEx = new Exception($"Sql fails:\r\n{command.CommandText}\r\n{ex.Message}", ex);
                throw newEx;
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }

            return result;
        }
    }
}