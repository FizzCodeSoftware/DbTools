namespace FizzCode.DbTools.DataDefinitionExecuter
{
    using System;
    using System.Configuration;
    using System.Data.Common;
    using System.Data.SQLite;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionGenerator;

#pragma warning disable CA1001 // Types that own disposable fields should be disposable
    public class SqLiteExecuter : SqlExecuter, ISqlExecuterDropAndCreateDatabase
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
    {
        public SqLiteExecuter(ConnectionStringSettings connectionStringSettings, ISqlGenerator sqlGenerator = null)
            : base(connectionStringSettings, sqlGenerator)
        {
        }

        private SQLiteConnection _connection;

        protected override SqlDialect SqlDialect => SqlDialect.SqLite;

        public override void InitializeDatabase(bool dropIfExists, params DatabaseDefinition[] dd)
        {
            if (dropIfExists)
                DropDatabase();

            CreateDatabase();
        }

        public void CreateDatabase()
        {
            if (_connection != null)
                throw new Exception("Database already connected.");

            _connection = new SQLiteConnection(ConnectionString);
            _connection.Open();
        }

        public override void CleanupDatabase(params DatabaseDefinition[] dds)
        {
            DropDatabase();
        }

        public void DropDatabase()
        {
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
                    var newEx = new Exception($"Sql fails:\r\n{command.CommandText}\r\n{ex.Message}", ex);
                    throw newEx;
                }
            }
        }

        public override Reader ExecuteQuery(SqlStatementWithParameters sqlStatementWithParameters)
        {
            using (var command = PrepareSqlCommand(sqlStatementWithParameters))
            {
                var reader = new Reader();
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

                            reader.Rows.Add(row);
                        }

                        return reader;
                    }
                    catch (SQLiteException ex)
                    {
                        var newEx = new Exception($"Sql fails:\r\n{command.CommandText}\r\n{ex.Message}", ex);
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
            using (var command = PrepareSqlCommand(sqlStatementWithParameters))
            {
                try
                {
                    var result = command.ExecuteScalar();
                    return result;
                }
                catch (SQLiteException ex)
                {
                    var newEx = new Exception($"Sql fails:\r\n{command.CommandText}\r\n{ex.Message}", ex);
                    throw newEx;
                }
            }
        }

        public override string GetDatabase(DbConnectionStringBuilder builder)
        {
            throw new NotImplementedException();
        }
    }
}