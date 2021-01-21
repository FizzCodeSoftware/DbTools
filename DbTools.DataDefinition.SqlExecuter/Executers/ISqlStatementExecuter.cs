namespace FizzCode.DbTools.DataDefinition.SqlExecuter
{
    using System.Data.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.SqlGenerator;
    using FizzCode.LightWeight.AdoNet;

    public interface ISqlStatementExecuter
    {
        NamedConnectionString ConnectionString { get; }
        ISqlGenerator Generator { get; }

        void ExecuteNonQuery(SqlStatementWithParameters sqlStatementWithParameters);
        RowSet ExecuteQuery(SqlStatementWithParameters sqlStatementWithParameters);
        object ExecuteScalar(SqlStatementWithParameters sqlStatementWithParameters);
        DbConnectionStringBuilder GetConnectionStringBuilder();
        string GetDatabase();
        DbConnection OpenConnection();
        DbConnection OpenConnectionMaster();
        DbCommand PrepareSqlCommand(SqlStatementWithParameters sqlStatementWithParameters);
        void InitializeDatabase(bool dropIfExists, params DatabaseDefinition[] dds);
        void CleanupDatabase(bool hard = false, params DatabaseDefinition[] dds);
    }
}