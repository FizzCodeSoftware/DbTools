namespace FizzCode.DbTools.Console
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Linq;
    using FizzCode.DbTools.Common;
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

                        var flags = (args.Length > 4)
                            ? args[4].Trim().Split(',').Select(x => (DocumenterFlags)Enum.Parse(typeof(DocumenterFlags), x)).Distinct().ToHashSet()
                            : new HashSet<DocumenterFlags>();

                        Document(args[1], (SqlDialect)Enum.Parse(typeof(SqlDialect), args[2]), patternFileName, flags);
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

        public static void Document(string connectionString, SqlDialect sqlDialect, string patternFileName, HashSet<DocumenterFlags> flags, IDocumenterWriter documenterWriter = null)
        {
            var connectionStringSettings = new ConnectionStringSettings
            {
                ConnectionString = connectionString,

                ProviderName = SqlDialectHelper.GetProviderNameFromSqlDialect(sqlDialect)
            };

            // TODO provider-specific ConnectionStringBuilder class
            var builder = new SqlConnectionStringBuilder(connectionString);
            var databaseName = builder.InitialCatalog;

            // TODO accept from argument
            var settings = Helper.GetDefaultSettings(sqlDialect);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(connectionStringSettings, settings);

            var dd = ddlReader.GetDatabaseDefinition();

            ITableCustomizer customizer = null;

            if (patternFileName != null)
                customizer = PatternMatchingTableCustomizerFromPatterns.FromCsv(patternFileName);

            var documenter = documenterWriter == null
                ? new Documenter(settings, databaseName, customizer, null, flags)
                : new Documenter(documenterWriter, settings, databaseName, customizer, null, flags);

            documenter.Document(dd);
        }

        public static void Generate(string connectionString, SqlDialect sqlDialect, string @namespace, string newDatabaseName, string patternFileName)
        {
            var connectionStringSettings = new ConnectionStringSettings
            {
                ConnectionString = connectionString,

                ProviderName = SqlDialectHelper.GetProviderNameFromSqlDialect(sqlDialect)
            };

            var settings = Helper.GetDefaultSettings(sqlDialect);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(connectionStringSettings, settings);

            var dd = ddlReader.GetDatabaseDefinition();

            ITableCustomizer customizer = null;

            if (patternFileName != null)
                customizer = PatternMatchingTableCustomizerFromPatterns.FromCsv(patternFileName);

            var generator = new CsGenerator(settings, newDatabaseName, @namespace, customizer);

            generator.GenerateMultiFile(dd, ConfigurationManager.AppSettings["WorkingDirectory"]);
        }
    }
}
