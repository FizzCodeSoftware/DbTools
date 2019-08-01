namespace FizzCode.DbTools.Console
{
    using System;
    using System.Configuration;
    using System.Data.SqlClient;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionDocumenter;
    using FizzCode.DbTools.DataDefinitionReader;

    public class Program
    {
        public static void Main(string[] args)
        {
            var command = args[0];

            switch (command.ToLower())
            {
                case "doc":
                    Document(args[1], (SqlDialect)Enum.Parse(typeof(SqlDialect), args[2]));
                    break;
                default:
                    Console.WriteLine("Unknown command");
                    break;
            }
        }

        public static void Document(string connectionString, SqlDialect sqlDialect)
        {
            var connectionStringSettings = new ConnectionStringSettings();
            connectionStringSettings.ConnectionString = connectionString;

            connectionStringSettings.ProviderName = SqlDialectHelper.GetProviderNameFromSqlDialect(sqlDialect);

            // TODO provider-specific ConnectionStringBuilder class
            var builder = new SqlConnectionStringBuilder(connectionString);

            var databaseName = builder.InitialCatalog;

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(connectionStringSettings);

            var dd = ddlReader.GetDatabaseDefinition();

            var documenter = new Documenter(databaseName);
            documenter.Document(dd);
        }
    }
}
