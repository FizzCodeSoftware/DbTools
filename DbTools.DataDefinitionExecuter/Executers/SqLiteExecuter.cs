namespace FizzCode.DbTools.DataDefinitionExecuter
{
    using System;
    using System.Configuration;
    using System.Data.SQLite;
    using System.Text.RegularExpressions;
    using FizzCode.DbTools.DataDefinitionGenerator;

    public class SqLiteExecuter : SqlExecuter
    {
        public SqLiteExecuter(ConnectionStringSettings connectionStringSettings, ISqlGenerator sqlGenerator = null)
            : base(connectionStringSettings, sqlGenerator)
        {
        }

        protected SQLiteConnection _connection;

        public override void CreateDatabase(bool shouldSkipIfExists)
        {
            if (!shouldSkipIfExists && _connection != null)
                throw new Exception("Database already connected.");

            _connection = new SQLiteConnection(ConnectionString);
            _connection.Open();
        }

        public override void DropDatabase()
        {
            if (_connection != null)
            {
                if(_connection.State != System.Data.ConnectionState.Closed)
                    _connection.Close();

                _connection = null;
            }
        }

        public override void DropDatabaseIfExists()
        {
            DropDatabase();
        }

        public SQLiteCommand PrepareSqlCommand(SqlStatementWithParameters sqlStatementWithParameters)
        {
            var command = _connection.CreateCommand();
            command.CommandText = sqlStatementWithParameters.Statement;

            foreach (var parameters in sqlStatementWithParameters.Parameters)
            {
                command.Parameters.AddWithValue(parameters.Key, parameters.Value);
            }

            return command;
        }

        public override void ExecuteNonQuery(SqlStatementWithParameters sqlStatementWithParameters)
        {
            try
            {
                using (var command = PrepareSqlCommand(sqlStatementWithParameters))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch (SQLiteException ex)
            {
                var newEx = new Exception($"Sql fails:\r\n{sqlStatementWithParameters.Statement}\r\n{ex.Message}", ex);
                throw newEx;
            }
        }

        public override Reader ExecuteQuery(SqlStatementWithParameters sqlStatementWithParameters)
        {
            try
            {
                var reader = new Reader();

                using (var sqlReader = PrepareSqlCommand(sqlStatementWithParameters).ExecuteReader())
                {
                    while (sqlReader.Read())
                    {
                        var row = new Row();
                        for (var i = 0; i < sqlReader.FieldCount; i++)
                        {
                            row.Add(sqlReader.GetName(i), sqlReader[i]);
                        }

                        reader.Rows.Add(row);
                    }
                }

                return reader;
            }
            catch (SQLiteException ex)
            {
                var newEx = new Exception($"Sql fails:\r\n{sqlStatementWithParameters.Statement}\r\n{ex.Message}", ex);
                throw newEx;
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
            try
            {
                using (var command = PrepareSqlCommand(sqlStatementWithParameters))
                {
                    var result = command.ExecuteScalar();
                    return result;
                }
            }
            catch (SQLiteException ex)
            {
                var newEx = new Exception($"Sql fails:\r\n{sqlStatementWithParameters.Statement}\r\n{ex.Message}", ex);
                throw newEx;
            }
        }

        protected override string ChangeInitialCatalog(string connectionString)
        {
            throw new NotImplementedException();
        }
    }
}