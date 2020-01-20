namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Common.Logger;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;

    public static class GeneratorColumnsFactory
    {

        public static GeneratorColumns CreateGeneratorColumns(SqlVersion version, Context context)
        {
            if (version is IGenericDialect)
                return new GenericGeneratorColumns1(context);

            if (version is ISqLiteDialect)
                return new SqLiteCsGeneratorColumns3(context);

            if (version is IMsSqlDialect)
                return new MsSqlCsGeneratorColumns2016(context);

            if (version is IOracleDialect)
                return new OracleCsGeneratorColumns12c(context);

            throw new NotImplementedException($"Not implemented {version}.");
        }
    }

    public class CsGenerator : DocumenterBase
    {
        private readonly string _namespace;

        private readonly GeneratorColumns _generatorColumns;

        public CsGenerator(DocumenterContext context, SqlVersion version, string databaseName, string @namespace)
            : base(context, version, databaseName)
        {
            _namespace = @namespace;
            // TODO handle versions
            _generatorColumns = GeneratorColumnsFactory.CreateGeneratorColumns(Version, context);
        }

        public void GenerateMultiFile(DatabaseDefinition databaseDefinition)
        {
            Log(LogSeverity.Information, "Starting on {DatabaseName}.", "CsGenerator", DatabaseName);

            var sb = new StringBuilder();
            WritePartialMainClassHeader(sb);
            sb.AppendLine("}");

            var folder = Path.Combine(Context.DocumenterSettings.WorkingDirectory ?? @".\", DatabaseName);
            Directory.CreateDirectory(folder);
            File.WriteAllText(Path.Combine(folder, DatabaseName + ".cs"), sb.ToString(), Encoding.UTF8);

            var sqlTablesByCategory = new List<KeyValuePair<string, SqlTable>>();
            foreach (var table in databaseDefinition.GetTables())
            {
                if (!Context.Customizer.ShouldSkip(table.SchemaAndTableName))
                {
                    sqlTablesByCategory.Add(new KeyValuePair<string, SqlTable>(Context.Customizer.Category(table.SchemaAndTableName), table));
                }
            }

            var tables = sqlTablesByCategory
                .OrderBy(kvp => kvp.Key)
                .ThenBy(t => t.Value.SchemaAndTableName.SchemaAndName);

            foreach (var tableKvp in tables)
            {
                var category = tableKvp.Key;
                var table = tableKvp.Value;

                sb.Clear();

                WritePartialTableFileHeader(sb);
                GenerateTable(sb, table);
                WritePartialTableFileFooter(sb);

                var categoryInPath = category;
                if (string.IsNullOrEmpty(categoryInPath))
                    categoryInPath = "_no_category_";

                categoryInPath = categoryInPath.Replace('?', '？');

                folder = Path.Combine(Context.DocumenterSettings.WorkingDirectory ?? @".\", DatabaseName, categoryInPath);

                var fileName = Helper.GetSimplifiedSchemaAndTableName(table.SchemaAndTableName, ".") + ".cs";

                Context.Logger.Log(LogSeverity.Information, "Writing Document file {FileName} to folder {Folder}", "Documenter", fileName, folder);

                Directory.CreateDirectory(folder);
                File.WriteAllText(Path.Combine(folder, fileName), sb.ToString(), Encoding.UTF8);
            }
        }

        public void GenerateSingleFile(DatabaseDefinition databaseDefinition, string fileName)
        {
            var sb = new StringBuilder();
            WriteSingleFileHeader(sb);

            var tables = databaseDefinition
                .GetTables()
                .Where(x => !Context.Customizer.ShouldSkip(x.SchemaAndTableName))
                .OrderBy(x => x.SchemaAndTableName.Schema)
                .ThenBy(x => x.SchemaAndTableName.TableName);

            var index = 0;
            foreach (var table in tables)
            {
                sb.AppendLine();
                GenerateTable(sb, table);
                index++;
            }

            WriteSingleFileFooter(sb);
            File.WriteAllText(fileName, sb.ToString(), Encoding.UTF8);
        }

        private void WritePartialMainClassHeader(StringBuilder sb)
        {
            sb.Append("namespace ")
            .AppendLine(_namespace)
            .AppendLine("{")
            .AppendLine(1, "using FizzCode.DbTools.DataDefinition;")
            .AppendLine()
            .Append(1, "public partial class ")
            .Append(DatabaseName)
            .AppendLine(" : DatabaseDeclaration")
            .AppendLine(1, "{")
            .AppendLine(1, "}");
        }

        private void WriteSingleFileHeader(StringBuilder sb)
        {
            sb.Append("namespace ")
            .AppendLine(_namespace)
            .AppendLine("{")
            .AppendLine("\tusing FizzCode.DbTools.DataDefinition;")
            .AppendLine()
            .Append(1, "public class ")
            .Append(DatabaseName)
            .AppendLine(" : DatabaseDeclaration")
            .AppendLine(1, "{")
            .AppendLine(2, "public " + DatabaseName + "(string defaultSchema = null, NamingStrategies namingStrategies = null)")
            .AppendLine(3, ": base(defaultSchema, namingStrategies)")
            .AppendLine(2, "{")
            .AppendLine(2, "}");
        }

        private static void WriteSingleFileFooter(StringBuilder sb)
        {
            sb.AppendLine(1, "}")
                .Append("}");
        }

        private void WritePartialTableFileHeader(StringBuilder sb)
        {
            sb.Append("namespace ")
                .AppendLine(_namespace)
                .AppendLine("{")
                .AppendLine(1, "using FizzCode.DbTools.DataDefinition;")
                .AppendLine()
                .Append(1, "public partial class ").AppendLine(DatabaseName)
                .AppendLine(1, "{");
        }

        private static void WritePartialTableFileFooter(StringBuilder sb)
        {
            sb.AppendLine(1, "}")
                .AppendLine("}");
        }

        protected void GenerateTable(StringBuilder sb, SqlTable table)
        {
            var pks = table.Properties.OfType<PrimaryKey>().ToList();
            if (pks.Count == 0)
            {
                sb.AppendLine(2, "// no primary key");
            }

            // TODO
            // - format schema and table name
            // - configure use of default schema
            sb
                .Append(2, "public SqlTable ")
                .Append(Helper.GetSimplifiedSchemaAndTableName(table.SchemaAndTableName, DatabaseDeclaration.SchemaTableNameSeparator.ToString(CultureInfo.InvariantCulture)))
                .AppendLine(" { get; } = AddTable((table) =>")
                .AppendLine(2, "{");

            var pkColumns = table.Columns
                .Where(column => column.Table.Properties.OfType<PrimaryKey>().Any(x => x.SqlColumns.Any(y => y.SqlColumn == column)))
                .ToList();

            foreach (var column in pkColumns)
            {
                var columnCreation = _generatorColumns.GetColumnCreation(column); 
                sb.AppendLine(columnCreation);
            }

            var regularColumns = table.Columns
                .Where(x => !pkColumns.Contains(x))
                .ToList();

            foreach (var column in regularColumns)
            {
                var columnCreation = _generatorColumns.GetColumnCreation(column);
                sb.AppendLine(columnCreation);
            }

            // TODO Indexes + config
            sb.AppendLine(2, "});");
        }
    }
}
