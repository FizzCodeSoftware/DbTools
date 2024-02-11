using System;
using System.Data.Common;
using FizzCode.DbTools.Common;
using FizzCode.DbTools.Common.Logger;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.Interfaces;
using FizzCode.LightWeight;

namespace FizzCode.DbTools.SqlExecuter;
public abstract class SqlStatementExecuter : ISqlStatementExecuter
{
    public ContextWithLogger Context { get; }

    public NamedConnectionString ConnectionString { get; }
    public ISqlGenerator Generator { get; }

    protected SqlStatementExecuter(ContextWithLogger context, NamedConnectionString connectionString, ISqlGenerator sqlGenerator)
    {
        Context = context;
        Generator = sqlGenerator;
        ConnectionString = connectionString;
    }

    protected abstract void ExecuteNonQueryMaster(SqlStatementWithParameters sqlStatementWithParameters);

    public DbConnection OpenConnection()
    {
        // TODO log conn string without password?
        Log(LogSeverity.Verbose, "Opening connection to {Database}.", GetDatabase());
        var dbf = DbProviderFactories.GetFactory(ConnectionString.ProviderName);

        var connection = dbf.CreateConnection();
        Throw.InvalidOperationExceptionIfNull(connection);
        connection.ConnectionString = ConnectionString.ConnectionString;
        connection.Open();

        return connection;
    }

    public virtual DbConnection OpenConnectionMaster()
    {
        var dbf = DbProviderFactories.GetFactory(ConnectionString.ProviderName);

        var connection = dbf.CreateConnection();
        Throw.InvalidOperationExceptionIfNull(connection);
        connection.Open();

        return connection;
    }

    public virtual DbCommand PrepareSqlCommand(SqlStatementWithParameters sqlStatementWithParameters)
    {
        var dbf = DbProviderFactories.GetFactory(ConnectionString.ProviderName);

        var command = dbf.CreateCommand();
        Throw.InvalidOperationExceptionIfNull(command);
#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
        command.CommandText = sqlStatementWithParameters.Statement;
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities

        foreach (var parameter in sqlStatementWithParameters.Parameters)
        {
            var dbParameter = command.CreateParameter();
            dbParameter.ParameterName = parameter.Key;
            dbParameter.Value = parameter.Value;
            if (parameter.Value is null)
                dbParameter.Value = DBNull.Value;
            command.Parameters.Add(dbParameter);
        }

        return command;
    }

    public DbConnectionStringBuilder GetConnectionStringBuilder()
    {
        var dbf = DbProviderFactories.GetFactory(ConnectionString.ProviderName);
        var csb = dbf.CreateConnectionStringBuilder();
        Throw.InvalidOperationExceptionIfNull(csb);
        return csb;
    }

    public abstract string GetDatabase();

    public abstract void InitializeDatabase(bool dropIfExists, params IDatabaseDefinition[] dds);

    public abstract void CleanupDatabase(bool hard = false, params IDatabaseDefinition[] dds);

    public virtual void ExecuteNonQuery(SqlStatementWithParameters sqlStatementWithParameters)
    {
        var connection = OpenConnection();

        var logTimer = LogTimer("Executing non query {Query}.", sqlStatementWithParameters.Statement);

        var command = PrepareSqlCommand(sqlStatementWithParameters);
        command.Connection = connection;
        command.CommandTimeout = 60 * 60;
        try
        {
            command.ExecuteNonQuery();
            logTimer.Done();
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

    public virtual RowSet ExecuteQuery(SqlStatementWithParameters sqlStatementWithParameters)
    {
        var connection = OpenConnection();

        //Log(LogSeverity.Verbose, "Executing query {Query}.", sqlStatementWithParameters.Statement);
        var logTimer = LogTimer("Executing query {Query}.", sqlStatementWithParameters.Statement);

        var command = PrepareSqlCommand(sqlStatementWithParameters);
        command.Connection = connection;

        try
        {
            var rowSet = new RowSet();
            using (var sqlReader = command.ExecuteReader())
            {
                var rowCount = 0;
                while (sqlReader.Read())
                {
                    rowCount++;
                    var row = new Row();
                    for (var i = 0; i < sqlReader.FieldCount; i++)
                    {
                        row.Add(sqlReader.GetName(i), sqlReader[i]);
                    }

                    rowSet.Add(row);
                }

                logTimer.Done();
                Log(LogSeverity.Verbose, "{rowCount} rows returned", rowCount);
            }

            logTimer.Done();

            return rowSet;
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

        // Log(LogSeverity.Verbose, "Executing scalar {Query}.", sqlStatementWithParameters.Statement);
        var logTimer = LogTimer("Executing scalar {Query}.", sqlStatementWithParameters.Statement);

        var command = PrepareSqlCommand(sqlStatementWithParameters);
        command.Connection = connection;

        try
        {
            var nullableResult = command.ExecuteScalar();
            result = nullableResult is null ? "" : nullableResult;
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

        logTimer.Done();

        return result;
    }

    protected void Log(LogSeverity severity, string text, params object[] args)
    {
        var module = "Executer/" + Generator.SqlVersion.UniqueName;
        Context.Logger.Log(severity, text, module, args);
    }

    protected LogTimer LogTimer(LogSeverity severity, string text, params object[] args)
    {
        var module = "Executer/" + Generator.SqlVersion.UniqueName;
        var logTimer = new LogTimer(Context.Logger, severity, text, module, args);
        return logTimer;
    }

    protected LogTimer LogTimer(string text, params object[] args)
    {
        return LogTimer(LogSeverity.Verbose, text, args);
    }
}