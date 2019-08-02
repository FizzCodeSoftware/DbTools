namespace FizzCode.DbTools.Console
{
    using System;
    using System.Configuration;
    using System.Data.SqlClient;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionDocumenter;
    using FizzCode.DbTools.DataDefinitionReader;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var command = args[0];

            switch (command.ToLower())
            {
                case "doc":
                    {
                        string patternFileName = null;
                        if (args.Length > 3)
                            patternFileName = args[3];
                        Document(args[1], (SqlDialect)Enum.Parse(typeof(SqlDialect), args[2]), patternFileName);
                        break;
                    }
                default:
                    Console.WriteLine("Unknown command");
                    break;
            }
        }

        public static void Document(string connectionString, SqlDialect sqlDialect, string patternFileName)
        {
            var connectionStringSettings = new ConnectionStringSettings();
            connectionStringSettings.ConnectionString = connectionString;

            connectionStringSettings.ProviderName = SqlDialectHelper.GetProviderNameFromSqlDialect(sqlDialect);

            // TODO provider-specific ConnectionStringBuilder class
            var builder = new SqlConnectionStringBuilder(connectionString);

            var databaseName = builder.InitialCatalog;

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(connectionStringSettings);

            var dd = ddlReader.GetDatabaseDefinition();

            ITableCustomizer customizer = null;

            if (patternFileName != null)
                customizer = new PatternMatchingTableCustomizerFromCsv(patternFileName);

            var documenter = new Documenter(databaseName, customizer);
            documenter.Document(dd);
        }
    }
}
