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
        public abstract void ExecuteNonQuery(string query, params object[] paramValues);
        public abstract Reader ExecuteQuery(string query, params object[] paramValues);
        protected abstract void ExecuteNonQueryMaster(string query, params object[] paramValues);
        public abstract object ExecuteScalar(string query, params object[] paramValues);
        protected abstract string ChangeInitialCatalog(string connectionString);
    }
}