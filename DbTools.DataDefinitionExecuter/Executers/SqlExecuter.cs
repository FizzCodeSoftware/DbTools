namespace FizzCode.DbTools.DataDefinitionExecuter
{
    using System.Configuration;
    using FizzCode.DbTools.DataDefinitionGenerator;

    public abstract class SqlExecuter
    {
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
        public abstract void DropDatabase();
        public abstract void DropDatabaseIfExists();
        public abstract void ExecuteNonQuery(SqlStatementWithParameters sqlStatementWithParameters);
        public abstract Reader ExecuteQuery(SqlStatementWithParameters sqlStatementWithParameters);
        protected abstract void ExecuteNonQueryMaster(SqlStatementWithParameters sqlStatementWithParameters);
        public abstract object ExecuteScalar(SqlStatementWithParameters sqlStatementWithParameters);
        protected abstract string ChangeInitialCatalog(string connectionString);
    }
}