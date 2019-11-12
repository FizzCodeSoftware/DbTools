namespace FizzCode.DbTools.Console
{
    using System.Collections.Generic;
    using CommandDotNet.Attributes;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Common.Logger;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionDocumenter;
    using FizzCode.DbTools.DataDefinitionExecuter;
    using FizzCode.DbTools.DataDefinitionGenerator;
    using FizzCode.DbTools.DataDefinitionReader;
    using Microsoft.Extensions.Configuration;

    [ApplicationMetadata(Name = ">")]
#pragma warning disable CA1812
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
            List<DocumenterFlag> flags)
        {
            var connectionStringWithProvider = new ConnectionStringWithProvider(sqlDialect.ToString(), SqlDialectHelper.GetProviderNameFromSqlDialect(sqlDialect), connectionString);

            // TODO provider-specific ConnectionStringBuilder class
            var sqlExecuter = SqlExecuterFactory.CreateSqlExecuter(connectionStringWithProvider, null);
            var databaseName = sqlExecuter.GetDatabase();

            // TODO accept from argument
            var settings = Helper.GetDefaultSettings(sqlDialect);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(connectionStringWithProvider, settings);

            var dd = ddlReader.GetDatabaseDefinition();

            var flagsSet = flags == null ? new HashSet<DocumenterFlag>() : new HashSet<DocumenterFlag>(flags);

            var context = CreateContext(settings, patternFileName);

            var documenter = new Documenter(context, databaseName, null, flagsSet);

            documenter.Document(dd);
        }

        [ApplicationMetadata(Name = "generate", Description = "Generate database definition into cs files.")]
        public void Generate(
            [Option(LongName = "connectionString", ShortName = "c")]
            string connectionString,
            [Option(LongName = "sqlDialect", ShortName = "d")]
            SqlDialect sqlDialect,
            [Option(LongName = "namespace", ShortName = "n")]
            string @namespace,
            [Option(LongName = "newDatabaseName", ShortName = "b")]
            string newDatabaseName,
            [Option(LongName = "patternFileName", ShortName = "p")]
            string patternFileName)
        {
            var connectionStringWithProvider = new ConnectionStringWithProvider
            (
                sqlDialect.ToString(),
                SqlDialectHelper.GetProviderNameFromSqlDialect(sqlDialect),
                connectionString
            );

            var settings = Helper.GetDefaultSettings(sqlDialect);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(connectionStringWithProvider, settings);

            var dd = ddlReader.GetDatabaseDefinition();

            ITableCustomizer customizer = null;

            var documenterSettings = Program.Configuration.GetSection("Documenter").Get<DocumenterSettings>();

            if (patternFileName != null)
                customizer = PatternMatchingTableCustomizerFromPatterns.FromCsv(patternFileName, documenterSettings);

            var context = CreateContext(settings, patternFileName);

            var generator = new CsGenerator(context, newDatabaseName, @namespace);

            generator.GenerateMultiFile(dd);
        }

        [ApplicationMetadata(Name = "bim", Description = "Generate database definition into bim (analysis services Model.bim xml) file.")]
        public void Bim(
            [Option(LongName = "connectionString", ShortName = "c")]
            string connectionString,
            [Option(LongName = "sqlDialect", ShortName = "d")]
            SqlDialect sqlDialect,
            [Option(LongName = "databaseName", ShortName = "b")]
            string databaseName,
            [Option(LongName = "patternFileName", ShortName = "p")]
            string patternFileName)
        {
            var connectionStringWithProvider = new ConnectionStringWithProvider
            (
                sqlDialect.ToString(),
                SqlDialectHelper.GetProviderNameFromSqlDialect(sqlDialect),
                connectionString
            );

            var settings = Helper.GetDefaultSettings(sqlDialect);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(connectionStringWithProvider, settings);

            var dd = ddlReader.GetDatabaseDefinition();

            var context = CreateContext(settings, patternFileName);

            var generator = new BimGenerator(context, databaseName);

            generator.Generate(dd);
        }

        private static Context CreateContext(Settings settings, string patternFileName)
        {
            var logger = new Logger();

            var iLogger = SerilogConfigurator.CreateLogger(null);
            var iOpsLogger = SerilogConfigurator.CreateOpsLogger(null);

            var consoleLogger = new ConsoleLogger();
            consoleLogger.Init(iLogger, iOpsLogger);

            logger.LogEvent += consoleLogger.OnLog;

            var documenterSettings = Program.Configuration.GetSection("Documenter").Get<DocumenterSettings>();

            ITableCustomizer customizer = null;

            if (patternFileName != null)
                customizer = PatternMatchingTableCustomizerFromPatterns.FromCsv(patternFileName, documenterSettings);

            customizer = customizer ?? new EmptyTableCustomizer();

            var context = new Context
            {
                DocumenterSettings = documenterSettings,
                Settings = settings,
                Logger = logger,
                Customizer = customizer
            };
            return context;
        }

        [ApplicationMetadata(Name = "dropall", Description = "Drop every object from a database.")]
        public void DropAll(
            [Option(LongName = "connectionString", ShortName = "c")]
            string connectionString,
            [Option(LongName = "sqlDialect", ShortName = "d")]
            SqlDialect sqlDialect
            )
        {
            var providerName = SqlDialectHelper.GetProviderNameFromSqlDialect(sqlDialect);
            var connectionStringWithProvider = new ConnectionStringWithProvider("", providerName, connectionString);
            var generator = SqlGeneratorFactory.CreateGenerator(sqlDialect, Helper.GetDefaultSettings(sqlDialect));
            var executer = SqlExecuterFactory.CreateSqlExecuter(connectionStringWithProvider, generator);
            var dc = new DatabaseCreator(null, executer);

            dc.DropAllViews();
            dc.DropAllForeignKeys();
            dc.DropAllTables();
            // TODO needs databasedefinition
            // dc.DropAllSchemas();
        }
    }
}
