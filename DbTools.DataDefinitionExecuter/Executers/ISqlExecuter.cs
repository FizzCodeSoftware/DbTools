namespace FizzCode.DbTools.DataDefinitionExecuter
{
    using System.Configuration;
    using System.Data.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionGenerator;

    public interface ISqlExecuter
    {
        string ConnectionString { get; }
        ConnectionStringSettings ConnectionStringSettings { get; }
        ISqlGenerator Generator { get; }

        void ExecuteNonQuery(SqlStatementWithParameters sqlStatementWithParameters);
        RowSet ExecuteQuery(SqlStatementWithParameters sqlStatementWithParameters);
        object ExecuteScalar(SqlStatementWithParameters sqlStatementWithParameters);
        DbConnectionStringBuilder GetConnectionStringBuilder();
        string GetDatabase(DbConnectionStringBuilder builder);
        DbConnection OpenConnection();
        DbConnection OpenConnectionMaster();
        DbCommand PrepareSqlCommand(SqlStatementWithParameters sqlStatementWithParameters);
        void InitializeDatabase(bool dropIfExists, params DatabaseDefinition[] dd);
        void CleanupDatabase(params DatabaseDefinition[] dds);
    }
}