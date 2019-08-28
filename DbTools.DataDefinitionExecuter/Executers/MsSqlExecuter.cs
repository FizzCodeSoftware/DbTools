namespace FizzCode.DbTools.DataDefinitionExecuter
{
    using System;
    using System.Configuration;
    using System.Data.Common;
    using System.Data.SqlClient;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionGenerator;

    public class MsSqlExecuter : SqlExecuter, ISqlExecuterDropAndCreateDatabase
    {
        protected override SqlDialect SqlDialect => SqlDialect.MsSql;

        public MsSqlExecuter(ConnectionStringSettings connectionStringSettings, ISqlGenerator sqlGenerator)
            : base(connectionStringSettings, sqlGenerator)
        {
        }

        public override void InitializeDatabase()
        {
            CreateDatabase();
        }

        public void CreateDatabase()
        {
            var builder = GetConnectionStringBuilder();
            builder.ConnectionString = ConnectionString;
            var sql = ((ISqlGeneratorDropAndCreateDatabase)Generator).CreateDatabase(GetDatabase(builder));
            ExecuteNonQueryMaster(sql);
        }

        public override void CleanupDatabase(params DatabaseDefinition[] dds)
        {
            DropDatabase();
        }

        public virtual void DropDatabaseIfExists()
        {
            var builder = GetConnectionStringBuilder();
            builder.ConnectionString = ConnectionString;
            var sql = ((ISqlGeneratorDropAndCreateDatabase)Generator).DropDatabaseIfExists(GetDatabase(builder));
            ExecuteNonQueryMaster(sql);
        }

        public virtual void DropDatabase()
        {
            var builder = GetConnectionStringBuilder();
            builder.ConnectionString = ConnectionString;
            var sql = ((ISqlGeneratorDropAndCreateDatabase)Generator).DropDatabase(GetDatabase(builder));
            ExecuteNonQueryMaster(sql);
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