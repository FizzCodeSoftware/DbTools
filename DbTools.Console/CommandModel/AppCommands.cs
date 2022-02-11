#pragma warning disable CA1812, CA1822
namespace FizzCode.DbTools.Console
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using CommandDotNet;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Common.Logger;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.SqlExecuter;
    using FizzCode.DbTools.DataDefinitionDocumenter;
    using FizzCode.LightWeight.AdoNet;
    using Microsoft.Extensions.Configuration;

    [Command(">")]
    internal class AppCommands
    {
        [Command("exit", Description = "Exit from the command-line utility.")]
        public void Exit()
        {
            Program.Terminated = true;
        }

        [Command("document", Description = "Generate excel documentation of an existing database")]
        public void Document(
            [Option('c', "connectionString")]
            string connectionString,
            [Option('t', "sqlType")]
            string sqlType,
            [Option('p', "patternFileName")]
            string patternFileName,
            [Option('f', "flags")]
            List<string> flags)
        {
            var version = SqlEngineVersions.GetVersion(sqlType);

            var connString = new NamedConnectionString(
                version.GetType().Name,
                version.ProviderName,
                connectionString,
                version.VersionString);

            var context = CreateContext(version);

            var sqlExecuter = SqlExecuterFactory.CreateSqlExecuter(connString, context);
            var databaseName = sqlExecuter.GetDatabase();

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(connString, context, null);

            var dd = ddlReader.GetDatabaseDefinition();

            var documenterContext = CreateDocumenterContext(context, patternFileName);
            if (flags != null)
                SetSettingsFromFlags(flags, documenterContext.DocumenterSettings);

            var documenter = new Documenter(documenterContext, version, databaseName, null);

            documenter.Document(dd);
        }

        [Command("generate", Description = "Generate database definition into cs files.")]
        public void Generate(
            [Option('c', "connectionString", Description = "Provide a valid connection string to the database")]
            string connectionString,
            [Option('m', "singleOrMulti", Description = "multi for multi file, single for single file generation")]
            string singleOrMulti,
            [Option('t', "sqlType")]
            string sqlType,
            [Option('n', "namespace")]
            string @namespace,
            [Option('b', "newDatabaseName")]
            string newDatabaseName,
            [Option('p', "patternFileName")]
            string patternFileName,
            [Option('f', "flags")]
            List<string> flags)
        {
            var version = SqlEngineVersions.GetVersion(sqlType);

            var connString = new NamedConnectionString(
                version.GetType().Name,
                version.ProviderName,
                connectionString,
                version.VersionString);

            var context = CreateContext(version);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(connString, context, null);

            var dd = ddlReader.GetDatabaseDefinition();

            var generatorContext = CreateGeneratorContext(context, patternFileName);
            if (flags != null)
                SetSettingsFromFlags(flags, generatorContext.GeneratorSettings);

            var writer = CSharpWriterFactory.GetCSharpWriter(version, generatorContext, newDatabaseName);
            var generator = new CSharpGenerator(writer, version, newDatabaseName, @namespace);

            if (singleOrMulti == "s" || singleOrMulti == "single")
                generator.GenerateSingleFile(dd, newDatabaseName + ".cs");
            else
                generator.GenerateMultiFile(dd);
        }

        [Command("generatetyped", Description = "Generate database definition into cs files with typed declaration.")]
        public void GenerateTyped(
            [Option('c', "connectionString", Description = "Provide a valid connection string to the database")]
            string connectionString,
            [Option('m', "singleOrMulti", Description = "multi for multi file, single for single file generation")]
            string singleOrMulti,
            [Option('t', "sqlType")]
            string sqlType,
            [Option('n', "namespace")]
            string @namespace,
            [Option('b', "newDatabaseName")]
            string newDatabaseName,
            [Option('p', "patternFileName")]
            string patternFileName,
            [Option('f', "flags")]
            List<string> flags)
        {
            var version = SqlEngineVersions.GetVersion(sqlType);

            var connString = new NamedConnectionString(
                version.GetType().Name,
                version.ProviderName,
                connectionString,
                version.VersionString);

            var context = CreateContext(version);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(connString, context, null);

            var dd = ddlReader.GetDatabaseDefinition();

            var generatorContext = CreateGeneratorContext(context, patternFileName);
            if (flags != null)
                SetSettingsFromFlags(flags, generatorContext.GeneratorSettings);

            var writer = CSharpTypedWriterFactory.GetCSharpTypedWriter(version, generatorContext, newDatabaseName);
            var generator = new CSharpTypedGenerator(writer, version, newDatabaseName, @namespace);

            if (singleOrMulti == "s" || singleOrMulti == "single")
                generator.GenerateSingleFile(dd, newDatabaseName + ".cs");
            else
                generator.GenerateMultiFile(dd);
        }

        private static void SetSettingsFromFlags(List<string> flagsAsStrings, DocumenterSettings settings)
        {
            foreach (var flagAsString in flagsAsStrings)
            {
                var flag = (DocumenterSettingFlag)System.Enum.Parse(typeof(DocumenterSettingFlag), flagAsString, true);

                if (flag == DocumenterSettingFlag.NoIndexes)
                    settings.NoIndexes = true;

                if (flag == DocumenterSettingFlag.NoUniqueConstraints)
                    settings.NoUniqueConstraints = true;

                if (flag == DocumenterSettingFlag.NoForeignKeys)
                    settings.NoForeignKeys = true;

                if (flag == DocumenterSettingFlag.NoInternalDataTypes)
                    settings.NoInternalDataTypes = true;
            }
        }

        private static void SetSettingsFromFlags(List<string> flagsAsStrings, GeneratorSettings settings)
        {
            foreach (var flagAsString in flagsAsStrings)
            {
                var flag = (GeneratorSettingFlag)System.Enum.Parse(typeof(GeneratorSettingFlag), flagAsString, true);

                if (flag == GeneratorSettingFlag.NoIndexes)
                    settings.NoIndexes = true;

                if (flag == GeneratorSettingFlag.NoUniqueConstraints)
                    settings.NoUniqueConstraints = true;

                if (flag == GeneratorSettingFlag.NoForeignKeys)
                    settings.NoForeignKeys = true;

                if (flag == GeneratorSettingFlag.ShouldCommentOutColumnsWithFkReferencedTables)
                    settings.ShouldCommentOutColumnsWithFkReferencedTables = true;

                if (flag == GeneratorSettingFlag.ShouldCommentOutFkReferences)
                    settings.ShouldCommentOutFkReferences = true;
            }
        }

        [Command("bim", Description = "Generate database definition into bim (analysis services Model.bim xml) file.")]
        public void Bim(
            [Option('c', "connectionString")]
            string connectionString,
            [Option('t', "sqlType")]
            string sqlType,
            [Option('b', "databaseName")]
            string databaseName,
            [Option('p', "patternFileName")]
            string patternFileName)
        {
            var version = SqlEngineVersions.GetVersion(sqlType);

            var connString = new NamedConnectionString(
                version.GetType().Name,
                version.ProviderName,
                connectionString,
                version.VersionString);

            var context = CreateContext(version);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(connString, context, null);

            var dd = ddlReader.GetDatabaseDefinition();

            var documenterContext = CreateDocumenterContext(context, patternFileName);

            var generator = new BimGenerator(documenterContext, version, databaseName);

            generator.Generate(dd);
        }

        [Command("check", Description = "")]
        public void Check(
            [Option('c', "connectionString")]
            string connectionString,
            [Option('t', "sqlType")]
            string sqlType,
            [Option('p', "patternFileName")]
            string patternFileName,
            [Option('f', "flags")]
            List<string> flags)
        {
            var version = SqlEngineVersions.GetVersion(sqlType);

            var connString = new NamedConnectionString(
                version.GetType().Name,
                version.ProviderName,
                connectionString,
                version.VersionString);

            var context = CreateContext(version);

            var sqlExecuter = SqlExecuterFactory.CreateSqlExecuter(connString, context);
            var databaseName = sqlExecuter.GetDatabase();

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(connString, context, null);

            var dd = ddlReader.GetDatabaseDefinition();

            var documenterContext = CreateDocumenterContext(context, patternFileName);
            if (flags != null)
                SetSettingsFromFlags(flags, documenterContext.DocumenterSettings);

            var schemaCheckerDocumenter = new SchemaCheckerDocumenter(documenterContext, version, databaseName, null);

            schemaCheckerDocumenter.Document(dd);
        }

        private static Logger CreateLogger()
        {
            var logger = new Logger();

            var logConfiguration = Program.Configuration.GetSection("Log").Get<LogConfiguration>();

            var iLogger = SerilogConfigurator.CreateLogger(logConfiguration);
            var iOpsLogger = SerilogConfigurator.CreateOpsLogger(logConfiguration);

            var consoleLogger = new ConsoleLogger();
            consoleLogger.Init(iLogger, iOpsLogger);

            logger.LogEvent += consoleLogger.OnLog;

            return logger;
        }

        private static Context CreateContext(SqlEngineVersion version)
        {
            var context = new Context
            {
                Logger = CreateLogger(),
                Settings = Helper.GetDefaultSettings(version, Program.Configuration)
            };

            return context;
        }

        private static DocumenterContext CreateDocumenterContext(Context context, string patternFileName)
        {
            var documenterSettings = Program.Configuration.GetSection("Documenter").Get<DocumenterSettings>();

            ITableCustomizer customizer = null;

            if (patternFileName != null)
                customizer = PatternMatchingTableCustomizerFromPatterns.FromCsv(patternFileName, documenterSettings);

            customizer ??= new EmptyTableCustomizer();

            var documenterContext = new DocumenterContext
            {
                DocumenterSettings = documenterSettings,
                Settings = context.Settings,
                Logger = context.Logger,
                Customizer = customizer
            };
            return documenterContext;
        }

        private static GeneratorContext CreateGeneratorContext(Context context, string patternFileName)
        {
            var documenterSettings = Program.Configuration.GetSection("Documenter").Get<DocumenterSettings>();

            var generatorSetting = new GeneratorSettings { WorkingDirectory = documenterSettings.WorkingDirectory };

            ITableCustomizer customizer = null;

            if (patternFileName != null)
                customizer = PatternMatchingTableCustomizerFromPatterns.FromCsv(patternFileName, documenterSettings);

            customizer ??= new EmptyTableCustomizer();

            var generatorContext = new GeneratorContext
            {
                GeneratorSettings = generatorSetting,
                Settings = context.Settings,
                Logger = context.Logger,
                Customizer = customizer
            };
            return generatorContext;
        }

        private static ChangeDocumenterContext CreateChangeDocumenterContext(Context context, string patternFileNameOriginal, string patternFileNameNew)
        {
            var documenterSettings = Program.Configuration.GetSection("Documenter").Get<DocumenterSettings>();

            ITableCustomizer customizerOriginal = null;
            ITableCustomizer customizerNew = null;

            if (patternFileNameOriginal != null)
                customizerOriginal = PatternMatchingTableCustomizerFromPatterns.FromCsv(patternFileNameOriginal, documenterSettings);

            if (patternFileNameNew != null)
                customizerNew = PatternMatchingTableCustomizerFromPatterns.FromCsv(patternFileNameNew, documenterSettings);

            customizerOriginal ??= new EmptyTableCustomizer();
            customizerNew ??= new EmptyTableCustomizer();

            var changeDocumenterContext = new ChangeDocumenterContext
            {
                DocumenterSettings = documenterSettings,
                Settings = context.Settings,
                Logger = context.Logger,
                CustomizerOriginal = customizerOriginal,
                CustomizerNew = customizerNew
            };
            return changeDocumenterContext;
        }

        [Command("dropall", Description = "Drop every object from a database.")]
        public void DropAll(
            [Option('c', "connectionString")]
            string connectionString,
            [Option('t', "sqlType")]
            string sqlType
            )
        {
            var version = SqlEngineVersions.GetVersion(sqlType);

            var connString = new NamedConnectionString("", version.ProviderName, connectionString, version.VersionString);

            var context = CreateContext(version);

            var generator = SqlGeneratorFactory.CreateGenerator(version, context);

            var executer = SqlExecuterFactory.CreateSqlExecuter(connString, generator);
            var dc = new DatabaseCreator(null, executer);

            dc.DropAllViews();
            dc.DropAllForeignKeys();
            dc.DropAllTables();
            // TODO needs databasedefinition
            // dc.DropAllSchemas();
        }

        [Command("changedocument", Description = "Generate compare excel documentation of two existing database")]
        public void ChangeDocument(
            [Option("connectionStringOriginal")]
            string connectionStringOriginal,
            [Option("connectionStringNew")]
            string connectionStringNew,
            [Option("sqlTypeOriginal")]
            string sqlTypeOriginal,
            [Option("sqlTypeNew")]
            string sqlTypeNew,
            [Option('p', "patternFileName")]
            string patternFileName,
            [Option("patternFileNameOriginal")]
            string patternFileNameOriginal,
            [Option("patternFileNameNew")]
            string patternFileNameNew,
            [Option('f', "flags")]
            List<string> flags)
        {
            var versionOriginal = SqlEngineVersions.GetVersion(sqlTypeOriginal);

            var contextOriginal = CreateContext(versionOriginal);

            var connString = new NamedConnectionString(
                versionOriginal.GetType().Name,
                versionOriginal.ProviderName,
                connectionStringOriginal,
                versionOriginal.VersionString);

            var sqlExecuterOriginal = SqlExecuterFactory.CreateSqlExecuter(connString, contextOriginal);
            var databaseNameOriginal = sqlExecuterOriginal.GetDatabase();

            var ddlReaderOriginal = DataDefinitionReaderFactory.CreateDataDefinitionReader(connString, contextOriginal, null);

            //var ddOriginal = ddlReaderOriginal.GetDatabaseDefinition();
            var ddOriginalTask = Task.Run(() => ddlReaderOriginal.GetDatabaseDefinition());

            if (patternFileNameOriginal == null)
                patternFileNameOriginal = patternFileName;

            if (patternFileNameNew == null)
                patternFileNameNew = patternFileName;

            var changeDocumenterContext = CreateChangeDocumenterContext(contextOriginal, patternFileNameOriginal, patternFileNameNew);
            if (flags != null)
                SetSettingsFromFlags(flags, changeDocumenterContext.DocumenterSettings);

            var versionNew = SqlEngineVersions.GetVersion(sqlTypeNew);

            var contextNew = CreateContext(versionNew);

            var connStringNew = new NamedConnectionString(
                versionNew.GetType().Name,
                versionNew.ProviderName,
                connectionStringNew,
                versionNew.VersionString);

            var sqlExecuterNew = SqlExecuterFactory.CreateSqlExecuter(connStringNew, contextNew);
            var databaseNameNew = sqlExecuterNew.GetDatabase();

            var ddlReaderNew = DataDefinitionReaderFactory.CreateDataDefinitionReader(connStringNew, contextNew, null);

            //var ddNew = ddlReaderNew.GetDatabaseDefinition();
            var ddNewTask = Task.Run(() => ddlReaderNew.GetDatabaseDefinition());

            var changeDocumenter = new ChangeDocumenter(changeDocumenterContext, versionOriginal, databaseNameOriginal, databaseNameNew);

            var ddOriginal = ddOriginalTask.Result;
            var ddNew = ddNewTask.Result;

            changeDocumenter.Document(ddOriginal, ddNew);
        }
    }
}
#pragma warning restore CA1812, CA1822