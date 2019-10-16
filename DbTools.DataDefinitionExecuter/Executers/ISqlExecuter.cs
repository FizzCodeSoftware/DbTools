namespace FizzCode.DbTools.DataDefinitionExecuter
{
    using System.Data.Common;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionGenerator;

    public interface ISqlExecuter
    {
        ConnectionStringWithProvider ConnectionStringWithProvider { get; }
        ISqlGenerator Generator { get; }

        void ExecuteNonQuery(SqlStatementWithParameters sqlStatementWithParameters);
        RowSet ExecuteQuery(SqlStatementWithParameters sqlStatementWithParameters);
        object ExecuteScalar(SqlStatementWithParameters sqlStatementWithParameters);
        DbConnectionStringBuilder GetConnectionStringBuilder();
        string GetDatabase();
        DbConnection OpenConnection();
        DbConnection OpenConnectionMaster();
        DbCommand PrepareSqlCommand(SqlStatementWithParameters sqlStatementWithParameters);
        void InitializeDatabase(bool dropIfExists, params DatabaseDefinition[] dd);
        void CleanupDatabase(params DatabaseDefinition[] dds);
    }
}