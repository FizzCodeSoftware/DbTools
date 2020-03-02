namespace FizzCode.DbTools.DataDefinition.MsSql2016
{
    using System;
    using System.Data.Common;
    using System.Data.SqlClient;
    using FizzCode.DbTools.Common.Logger;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.SqlExecuter;
    using FizzCode.DbTools.DataDefinition.SqlGenerator;

    public class MsSql2016Executer : SqlStatementExecuter, ISqlExecuterDropAndCreateDatabase
    {
        public MsSql2016Executer(ConnectionStringWithProvider connectionStringWithProvider, ISqlGenerator sqlGenerator)
            : base(connectionStringWithProvider, sqlGenerator)
        {
        }

        public override void InitializeDatabase(bool dropIfExists, params DatabaseDefinition[] dds)
        {
            DropDatabaseIfExists();
            CreateDatabase();
        }

        public void CreateDatabase()
        {
            var sql = ((ISqlGeneratorDropAndCreateDatabase)Generator).CreateDatabase(GetDatabase());
            ExecuteNonQueryMaster(sql);
        }

        public override void CleanupDatabase(bool hard = false, params DatabaseDefinition[] dds)
        {
            DropDatabase();
        }

        public virtual void DropDatabaseIfExists()
        {
            var sql = ((ISqlGeneratorDropAndCreateDatabase)Generator).DropDatabaseIfExists(GetDatabase());
            ExecuteNonQueryMaster(sql);
        }

        public virtual void DropDatabase()
        {
            var sql = ((ISqlGeneratorDropAndCreateDatabase)Generator).DropDatabase(GetDatabase());
            ExecuteNonQueryMaster(sql);
        }

        protected override void ExecuteNonQueryMaster(SqlStatementWithParameters sqlStatementWithParameters)
        {
            SqlConnection.ClearAllPools(); // force closing connections to normal database to be able to exetute DDLs.

            var connection = OpenConnectionMaster();
            Log(LogSeverity.Verbose, "Executing query {Query} on master.", sqlStatementWithParameters.Statement);

            var command = PrepareSqlCommand(sqlStatementWithParameters);
            command.Connection = connection;

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

        public override DbConnection OpenConnectionMaster()
        {
            Log(LogSeverity.Verbose, "Opening connection to Master.");
            var connection = new SqlConnection(ChangeInitialCatalog(""));
            connection.Open();

            return connection;
        }

        private string ChangeInitialCatalog(string newInitialCatalog)
        {
            var builder = GetConnectionStringBuilder();
            builder.ConnectionString = ConnectionStringWithProvider.ConnectionString;
            if (newInitialCatalog != null)
                builder[InitialCatalog] = newInitialCatalog;

            return builder.ConnectionString;
        }

        public const string InitialCatalog = "Initial Catalog";

        public override string GetDatabase()
        {
            var builder = GetConnectionStringBuilder();
            builder.ConnectionString = ConnectionStringWithProvider.ConnectionString;
            return builder.ValueOfKey<string>(InitialCatalog);
        }
    }
}