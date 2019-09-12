namespace FizzCode.DbTools.DataDefinitionExecuter
{
    using System;
    using System.Configuration;
    using System.Data.Common;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionGenerator;

    public abstract class SqlExecuter : ISqlExecuter
    {
        protected abstract SqlDialect SqlDialect { get; }

        public ConnectionStringSettings ConnectionStringSettings { get; }
        public ISqlGenerator Generator { get; }
        public string ConnectionString { get; }

        protected SqlExecuter(ConnectionStringSettings connectionStringSettings, ISqlGenerator sqlGenerator)
        {
            Generator = sqlGenerator;

            ConnectionStringSettings = connectionStringSettings;
            ConnectionString = ConnectionStringSettings.ConnectionString;
        }

        protected abstract void ExecuteNonQueryMaster(SqlStatementWithParameters sqlStatementWithParameters);

        public DbConnection OpenConnection()
        {
            var dbf = DbProviderFactories.GetFactory(SqlDialectHelper.GetProviderNameFromSqlDialect(SqlDialect));

            var connection = dbf.CreateConnection();
            connection.ConnectionString = ConnectionString;
            connection.Open();

            return connection;
        }

        public virtual DbConnection OpenConnectionMaster()
        {
            // TODO Oracle?
            var dbf = DbProviderFactories.GetFactory(SqlDialectHelper.GetProviderNameFromSqlDialect(SqlDialect));

            var connection = dbf.CreateConnection();
            connection.Open();

            return connection;
        }

        public virtual DbCommand PrepareSqlCommand(SqlStatementWithParameters sqlStatementWithParameters)
        {
            var dbf = DbProviderFactories.GetFactory(SqlDialectHelper.GetProviderNameFromSqlDialect(SqlDialect));

            var command = dbf.CreateCommand();
            command.CommandText = sqlStatementWithParameters.Statement;

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

        public abstract string GetDatabase(DbConnectionStringBuilder builder);

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

        public virtual Reader ExecuteQuery(SqlStatementWithParameters sqlStatementWithParameters)
        {
            var connection = OpenConnection();

            var command = PrepareSqlCommand(sqlStatementWithParameters);
            command.Connection = connection;

            try
            {
                var reader = new Reader();

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

        public Settings GetSettings()
        {
            return Generator.GetSettings();
        }
    }
}