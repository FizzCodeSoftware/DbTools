using System.Data.SQLite;
using FizzCode.DbTools.Common;
using FizzCode.DbTools.Common.Logger;
using FizzCode.LightWeight.AdoNet;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.Interfaces;

namespace FizzCode.DbTools.SqlExecuter.SqLite3;
#pragma warning disable CA1001 // Types that own disposable fields should be disposable
public class SqLite3Executer : SqlStatementExecuter, ISqlExecuterDropAndCreateDatabase
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
{
    public SqLite3Executer(ContextWithLogger context, NamedConnectionString connectionString, ISqlGenerator? sqlGenerator = null)
        : base(context, connectionString, sqlGenerator)
    {
    }

    private SQLiteConnection _connection;

    public override void InitializeDatabase(bool dropIfExists, params IDatabaseDefinition[] dds)
    {
        if (dropIfExists)
            DropDatabase();

        CreateDatabase();
    }

    public void CreateDatabase()
    {
        Log(LogSeverity.Verbose, "Create database.");

        if (_connection != null)
            throw new System.Exception("Database already connected.");

        _connection = new SQLiteConnection(ConnectionString.ConnectionString);
        _connection.Open();
    }

    public override void CleanupDatabase(bool hard = false, params IDatabaseDefinition[] dds)
    {
        DropDatabase();
    }

    public void DropDatabase()
    {
        Log(LogSeverity.Verbose, "Drop database.");

        if (_connection != null)
        {
            if (_connection.State != System.Data.ConnectionState.Closed)
                _connection.Close();

            _connection = null;
        }
    }

    public void DropDatabaseIfExists()
    {
        DropDatabase();
    }

    public override void ExecuteNonQuery(SqlStatementWithParameters sqlStatementWithParameters)
    {
        Log(LogSeverity.Verbose, "Executing non query {Query}.", sqlStatementWithParameters.Statement);

        using (var command = PrepareSqlCommand(sqlStatementWithParameters))
        {
            command.Connection = _connection;
            try
            {
                {
                    command.ExecuteNonQuery();
                }
            }
            catch (SQLiteException ex)
            {
                var newEx = new System.Exception($"Sql fails:\r\n{command.CommandText}\r\n{ex.Message}", ex);
                throw newEx;
            }
        }
    }

    public override RowSet ExecuteQuery(SqlStatementWithParameters sqlStatementWithParameters)
    {
        Log(LogSeverity.Verbose, "Executing query {Query}.", sqlStatementWithParameters.Statement);

        using (var command = PrepareSqlCommand(sqlStatementWithParameters))
        {
            command.Connection = _connection;
            var rowSet = new RowSet();
            using (var sqlReader = command.ExecuteReader())
            {
                try
                {
                    while (sqlReader.Read())
                    {
                        var row = new Row();
                        for (var i = 0; i < sqlReader.FieldCount; i++)
                        {
                            row.Add(sqlReader.GetName(i), sqlReader[i]);
                        }

                        rowSet.Add(row);
                    }

                    return rowSet;
                }
                catch (SQLiteException ex)
                {
                    var newEx = new System.Exception($"Sql fails:\r\n{command.CommandText}\r\n{ex.Message}", ex);
                    throw newEx;
                }
            }
        }
    }

    protected override void ExecuteNonQueryMaster(SqlStatementWithParameters sqlStatementWithParameters)
    {
        using (var command = PrepareSqlCommand(sqlStatementWithParameters))
        {
            command.ExecuteNonQuery();
        }
    }

    public override object ExecuteScalar(SqlStatementWithParameters sqlStatementWithParameters)
    {
        Log(LogSeverity.Verbose, "Executing scalar {Query}.", sqlStatementWithParameters.Statement);

        using (var command = PrepareSqlCommand(sqlStatementWithParameters))
        {
            try
            {
                var result = command.ExecuteScalar();
                return result;
            }
            catch (SQLiteException ex)
            {
                var newEx = new System.Exception($"Sql fails:\r\n{command.CommandText}\r\n{ex.Message}", ex);
                throw newEx;
            }
        }
    }

    public override string GetDatabase()
    {
        throw new System.NotImplementedException();
    }
}