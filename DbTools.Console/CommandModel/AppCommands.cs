namespace FizzCode.DbTools.Console
{
    using System.Collections.Generic;
    using System.Configuration;
    using CommandDotNet.Attributes;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionDocumenter;
    using FizzCode.DbTools.DataDefinitionExecuter;
    using FizzCode.DbTools.DataDefinitionReader;
    using Microsoft.Extensions.Configuration;

    [ApplicationMetadata(Name = ">")]
    internal class AppCommands
    {
        [ApplicationMetadata(Name = "exit", Description = "Exit from the command-line utility.")]
        public void Exit()
        {
            Program.Terminated = true;
        }

        [ApplicationMetadata(Name = "document", Description = "Generate excel documentation of an existing database")]
        public void Document(
            [Option(LongName = "connectionString", ShortName = "c")]
            string connectionString,
            [Option(LongName = "sqlDialect", ShortName = "d")]
            SqlDialect sqlDialect,
            [Option(LongName = "patternFileName", ShortName = "p")]
            string patternFileName,
            [Option(LongName = "flags", ShortName = "f")]
            List<DocumenterFlags> flags)
        {
            var connectionStringSettings = new ConnectionStringSettings
            {
                ConnectionString = connectionString,

                ProviderName = SqlDialectHelper.GetProviderNameFromSqlDialect(sqlDialect)
            };

            // TODO provider-specific ConnectionStringBuilder class
            var sqlExecuter = SqlExecuterFactory.CreateSqlExecuter(connectionStringSettings, null);
            var databaseName = sqlExecuter.GetDatabase();

            // TODO accept from argument
            var settings = Helper.GetDefaultSettings(sqlDialect);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(connectionStringSettings, settings);

            var dd = ddlReader.GetDatabaseDefinition();

            ITableCustomizer customizer = null;

            var documenterSettings = Program.Configuration.GetSection("Documenter").Get<DocumenterSettings>();

            if (patternFileName != null)
                customizer = PatternMatchingTableCustomizerFromPatterns.FromCsv(patternFileName, documenterSettings);

            var documenter = new Documenter(documenterSettings, settings, databaseName, customizer, null, new HashSet<DocumenterFlags>(flags));

            documenter.Document(dd);
        }

        [ApplicationMetadata(Name = "generate", Description = "Generate database definition into cs files.")]
        public static void Generate(
            [Option(LongName = "connectionString", ShortName = "c")]
            string connectionString,
            [Option(LongName = "sqlDialect", ShortName = "d")]
            SqlDialect sqlDialect,
            [Option(LongName = "namespace", ShortName = "ns")]
            string @namespace,
            [Option(LongName = "newDatabaseName", ShortName = "dbn")]
            string newDatabaseName,
            [Option(LongName = "patternFileName", ShortName = "p")]
            string patternFileName)
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

            var documenterSettings = Program.Configuration.GetSection("Documenter").Get<DocumenterSettings>();

            if (patternFileName != null)
                customizer = PatternMatchingTableCustomizerFromPatterns.FromCsv(patternFileName, documenterSettings);

            var generator = new CsGenerator(documenterSettings, settings, newDatabaseName, @namespace, customizer);

            generator.Generate(dd);
        }

        
    }
}
