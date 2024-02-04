using System.Data.Common;
using FizzCode.DbTools.Common;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.LightWeight.AdoNet;

namespace FizzCode.DbTools.Interfaces;
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
    void InitializeDatabase(bool dropIfExists, params IDatabaseDefinition[] dds);
    void CleanupDatabase(bool hard = false, params IDatabaseDefinition[] dds);
}