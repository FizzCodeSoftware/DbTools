namespace FizzCode.DbTools.DataDefinitionExecuter
{
    using System.Configuration;
    using System.Data.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionGenerator;

    public abstract class SqlExecuter
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

        public abstract void CreateDatabase(bool shouldSkipIfExists);
        public abstract void ExecuteNonQuery(SqlStatementWithParameters sqlStatementWithParameters);
        public abstract Reader ExecuteQuery(SqlStatementWithParameters sqlStatementWithParameters);
        protected abstract void ExecuteNonQueryMaster(SqlStatementWithParameters sqlStatementWithParameters);
        public abstract object ExecuteScalar(SqlStatementWithParameters sqlStatementWithParameters);

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

        public DbCommand PrepareSqlCommand(SqlStatementWithParameters sqlStatementWithParameters)
        {
            var dbf = DbProviderFactories.GetFactory(SqlDialectHelper.GetProviderNameFromSqlDialect(SqlDialect));

            var command = dbf.CreateCommand();
            command.CommandText = sqlStatementWithParameters.Statement;

            foreach (var parameter in sqlStatementWithParameters.Parameters)
            {
                var dbParameter = command.CreateParameter();
                dbParameter.ParameterName = parameter.Key;
                dbParameter.Value = parameter.Value;
            }

            return command;
        }

        public DbConnectionStringBuilder GetConnectionStringBuilder()
        {
            var dbf = DbProviderFactories.GetFactory(SqlDialectHelper.GetProviderNameFromSqlDialect(SqlDialect));
            return dbf.CreateConnectionStringBuilder();
        }

        public virtual void DropDatabaseIfExists()
        {
            var builder = GetConnectionStringBuilder();
            builder.ConnectionString = ConnectionString;
            var sql = Generator.DropDatabaseIfExists(GetDatabase(builder));
            ExecuteNonQueryMaster(sql);
        }

        public virtual void DropDatabase()
        {
            var builder = GetConnectionStringBuilder();
            builder.ConnectionString = ConnectionString;
            var sql = Generator.DropDatabase(GetDatabase(builder));
            ExecuteNonQueryMaster(sql);
        }

        public abstract string GetDatabase(DbConnectionStringBuilder builder);
    }
}