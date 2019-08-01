namespace FizzCode.DbTools.DataDefinitionExecuter
{
    using System;
    using System.Configuration;
    using System.Data.SQLite;
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

        public override void ExecuteNonQuery(string statement)
        {
            try
            {
                using (var cmd = _connection.CreateCommand())
                {
                    cmd.CommandText = statement;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SQLiteException ex)
            {
                var newEx = new Exception($"Sql fails:\r\n{statement}\r\n{ex.Message}", ex);
                throw newEx;
            }
        }

        public override Reader ExecuteQuery(string sql)
        {
            try
            {
                var reader = new Reader();
                using (var sqlReader = new SQLiteCommand(sql, _connection).ExecuteReader())
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

        protected override void ExecuteNonQueryMaster(string statement)
        {
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = statement;
                cmd.ExecuteNonQuery();
            }
        }

        public override object ExecuteScalar(string statement)
        {
            try
            {
                using (var cmd = _connection.CreateCommand())
                {
                    cmd.CommandText = statement;
                    var result = cmd.ExecuteScalar();
                    return result;
                }
            }
            catch (SQLiteException ex)
            {
                var newEx = new Exception($"Sql fails:\r\n{statement}\r\n{ex.Message}", ex);
                throw newEx;
            }
        }

        protected override string ChangeInitialCatalog(string connectionString)
        {
            throw new NotImplementedException();
        }
    }
}