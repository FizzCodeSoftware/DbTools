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
                            patternFileName = args[3].Trim();
                        Document(args[1], (SqlDialect)Enum.Parse(typeof(SqlDialect), args[2]), patternFileName);
                        break;
                    }
                case "gen":
                    {
                        string patternFileName = null;
                        if (args.Length > 5)
                            patternFileName = args[5].Trim();
                        Generate(args[1], (SqlDialect)Enum.Parse(typeof(SqlDialect), args[2]), args[3], args[4], patternFileName);
                        break;
                    }
                default:
                    Console.WriteLine("Unknown command");
                    break;
            }
        }

        public static void Document(string connectionString, SqlDialect sqlDialect, string patternFileName, IDocumenterWriter documenterWriter = null)
        {
            var connectionStringSettings = new ConnectionStringSettings
            {
                ConnectionString = connectionString,

                ProviderName = SqlDialectHelper.GetProviderNameFromSqlDialect(sqlDialect)
            };

            // TODO provider-specific ConnectionStringBuilder class
            var builder = new SqlConnectionStringBuilder(connectionString);

            var databaseName = builder.InitialCatalog;

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(connectionStringSettings);

            var dd = ddlReader.GetDatabaseDefinition();

            ITableCustomizer customizer = null;

            if (patternFileName != null)
                customizer = new PatternMatchingTableCustomizerFromCsv(patternFileName);

            var documenter = documenterWriter == null
                ? new Documenter(databaseName, customizer)
                : new Documenter(documenterWriter, databaseName, customizer);

            documenter.Document(dd);
        }

        public static void Generate(string connectionString, SqlDialect sqlDialect, string @namespace, string newDatabaseName, string patternFileName)
        {
            var connectionStringSettings = new ConnectionStringSettings
            {
                ConnectionString = connectionString,

                ProviderName = SqlDialectHelper.GetProviderNameFromSqlDialect(sqlDialect)
            };

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(connectionStringSettings);

            var dd = ddlReader.GetDatabaseDefinition();

            ITableCustomizer customizer = null;

            if (patternFileName != null)
                customizer = new PatternMatchingTableCustomizerFromCsv(patternFileName);

            var generator = new CsGenerator(newDatabaseName, @namespace, customizer);

            generator.Generate(dd);
        }
    }
}
