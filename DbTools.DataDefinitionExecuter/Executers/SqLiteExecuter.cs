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

        public SQLiteCommand PrepareSqlCommand(string sql, params object[] paramValues)
        {
            var command = _connection.CreateCommand();
            command.CommandText = sql;
            var matches = Regex.Matches(sql, @"\B\@\w+");
            var i = 0;

            foreach (var paramValue in paramValues)
            {
                command.Parameters.AddWithValue(matches[i++].Value, paramValue);
            }

            return command;
        }

        // TODO paramters
        public override void ExecuteNonQuery(string sql, params object[] paramValues)
        {
            try
            {
                using (var command = PrepareSqlCommand(sql, paramValues))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch (SQLiteException ex)
            {
                var newEx = new Exception($"Sql fails:\r\n{sql}\r\n{ex.Message}", ex);
                throw newEx;
            }
        }

        // TODO paramters
        public override Reader ExecuteQuery(string sql, params object[] paramValues)
        {
            try
            {
                var reader = new Reader();

                using (var sqlReader = PrepareSqlCommand(sql, paramValues).ExecuteReader())
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
                var newEx = new Exception($"Sql fails:\r\n{sql}\r\n{ex.Message}", ex);
                throw newEx;
            }
        }

        protected override void ExecuteNonQueryMaster(string sql, params object[] paramValues)
        {
            using (var command = PrepareSqlCommand(sql, paramValues))
            {
                command.ExecuteNonQuery();
            }
        }

        public override object ExecuteScalar(string sql, params object[] paramValues)
        {
            try
            {
                using (var command = PrepareSqlCommand(sql, paramValues))
                {
                    var result = command.ExecuteScalar();
                    return result;
                }
            }
            catch (SQLiteException ex)
            {
                var newEx = new Exception($"Sql fails:\r\n{sql}\r\n{ex.Message}", ex);
                throw newEx;
            }
        }

        protected override string ChangeInitialCatalog(string connectionString)
        {
            throw new NotImplementedException();
        }
    }
}