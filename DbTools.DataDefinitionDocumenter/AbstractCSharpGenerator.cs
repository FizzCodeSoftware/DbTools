namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Common.Logger;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Base;

    public abstract class AbstractCSharpGenerator : DocumenterBase
    {
        protected new GeneratorContext Context => (GeneratorContext)base.Context;

        private readonly string _namespace;
        protected AbstractCSharpWriterBase Writer { get; }

        public string[] AdditionalNamespaces { get; set; }
        public Func<SqlTable, string> OnTableComment { get; set; }
        public Func<SqlTable, string> OnTableAnnotation { get; set; }
        public Func<SqlColumn, string> OnColumnAnnotation { get; set; }
        public Func<SqlColumn, string> OnColumnComment { get; set; }

        protected AbstractCSharpGenerator(AbstractCSharpWriterBase writer, SqlEngineVersion version, string databaseName, string @namespace)
            : base(writer.GeneratorContext, version, databaseName)
        {
            _namespace = @namespace;
            Writer = writer;
        }

        public void GenerateMultiFile(IDatabaseDefinition databaseDefinition)
        {
            Log(LogSeverity.Information, "Starting on {DatabaseName}.", "CsGenerator", DatabaseName);

            var sb = new StringBuilder();
            WritePartialMainClassHeader(sb);
            sb.AppendLine("}");

            var folder = Path.Combine(Context.GeneratorSettings.WorkingDirectory ?? @".\", DatabaseName);
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

                folder = Path.Combine(Context.GeneratorSettings.WorkingDirectory ?? @".\", DatabaseName, categoryInPath);

                var fileName = Helper.GetSimplifiedSchemaAndTableName(table.SchemaAndTableName, ".") + ".cs";

                Context.Logger.Log(LogSeverity.Information, "Writing Document file {FileName} to folder {Folder}", "Documenter", fileName, folder);

                Directory.CreateDirectory(folder);
                File.WriteAllText(Path.Combine(folder, fileName), sb.ToString(), Encoding.UTF8);
            }
        }

        public void GenerateSingleFile(IDatabaseDefinition databaseDefinition, string fileName, bool partialClass = false)
        {
            Log(LogSeverity.Information, "Starting on {DatabaseName}.", "CsGenerator", DatabaseName);

            var sb = new StringBuilder();

            var tables = databaseDefinition
                .GetTables()
                .Where(x => Context.Customizer?.ShouldSkip(x.SchemaAndTableName) != true)
                .OrderBy(x => x.SchemaAndTableName.Schema)
                .ThenBy(x => x.SchemaAndTableName.TableName).ToList();

            WriteSingleFileHeader(sb, tables, partialClass);

            var index = 0;
            foreach (var table in tables)
            {
                sb.AppendLine();
                GenerateTable(sb, table);
                index++;
            }

            WriteSingleFileFooter(sb);

            Context.Logger.Log(LogSeverity.Information, "Writing Document file {FileName}", "Documenter", fileName);

            File.WriteAllText(fileName, sb.ToString(), Encoding.UTF8);
        }

        protected abstract void GenerateTable(StringBuilder sb, SqlTable table);

        private void WritePartialMainClassHeader(StringBuilder sb)
        {
            sb.Append("namespace ")
            .AppendLine(_namespace)
            .AppendLine("{")
            .AppendLine(1, "using FizzCode.DbTools.DataDefinition;")
            .AppendLine(1, "using FizzCode.DbTools.Configuration;")
            .AppendLine(1, "using " + Writer.GetSqlTypeNamespace() + ";");

            if (AdditionalNamespaces != null)
            {
                foreach (var line in AdditionalNamespaces)
                {
                    sb.Append(1, "using ").Append(line).AppendLine(";");
                }
            }

            sb.AppendLine()
            .Append(1, "public partial class ")
            .Append(DatabaseName)
            .AppendLine(" : DatabaseDeclaration")
            .AppendLine(1, "{")
            .AppendLine(1, "}");
        }

        protected abstract void WriteSingleFileHeader(StringBuilder sb, List<SqlTable> tables, bool partialClass = false);

        protected void WriteSingleFileHeaderCommon(StringBuilder sb, bool partialClass = false)
        {
            sb.Append("namespace ")
                .AppendLine(_namespace)
                .AppendLine("{");

            var usedNamespaces = new List<string>()
            {
                "FizzCode.DbTools.Configuration",
                "FizzCode.DbTools.DataDefinition",
                Writer.GetSqlTypeNamespace(),
            };

            if (Context.GeneratorSettings.ShouldUseStoredProceduresFromQueries)
                usedNamespaces.Add("FizzCode.DbTools.QueryBuilder");

            if (AdditionalNamespaces != null)
            {
                usedNamespaces.AddRange(AdditionalNamespaces);
            }

            usedNamespaces.Sort();

            foreach (var ns in usedNamespaces)
            {
                sb.Append(1, "using ").Append(ns).AppendLine(";");
            }

            sb.AppendLine()
            .Append(1, "public ")
            .Append(partialClass ? "partial " : "")
            .Append("class ")
            .Append(DatabaseName)
            .AppendLine(" : DatabaseDeclaration")
            .AppendLine(1, "{")
            .AppendLine(2, "public " + DatabaseName + "(string defaultSchema = null, NamingStrategies namingStrategies = null)")
            .Append(3, ": base(");

            if (Context.GeneratorSettings.ShouldUseStoredProceduresFromQueries)
                sb.Append("new QueryBuilder(), ");

            sb.Append("new ")
            .Append(string.Equals(Writer.TypeMapperType.Namespace, Writer.GetSqlTypeNamespace(), StringComparison.InvariantCultureIgnoreCase)
                ? Writer.TypeMapperType.Name
                : Writer.TypeMapperType.AssemblyQualifiedName)
            .AppendLine("(), null, defaultSchema, namingStrategies)")
            .AppendLine(2, "{")
            .AppendLine(2, "}");
        }

        protected abstract void WriteSingleFileFooter(StringBuilder sb);

        private void WritePartialTableFileHeader(StringBuilder sb)
        {
            sb.Append("namespace ")
                .AppendLine(_namespace)
                .AppendLine("{")
                .AppendLine(1, "using FizzCode.DbTools.DataDefinition;");

            if (AdditionalNamespaces != null)
            {
                foreach (var line in AdditionalNamespaces)
                {
                    sb.Append(1, "using ").Append(line).AppendLine(";");
                }
            }

            sb.AppendLine()
                .Append(1, "public partial class ").AppendLine(DatabaseName)
                .AppendLine(1, "{");
        }

        private static void WritePartialTableFileFooter(StringBuilder sb)
        {
            sb.AppendLine(1, "}")
                .AppendLine("}");
        }

        protected void GenerateTableProperties(StringBuilder sb, SqlTable table)
        {
            if (!Context.GeneratorSettings.NoIndexes)
            {
                var indexes = table.Properties.OfType<DataDefinition.Base.Index>().ToList();
                foreach (var index in indexes)
                    GenerateIndex(sb, index);
            }

            if (!Context.GeneratorSettings.NoUniqueConstraints)
            {
                var uniqueConstraints = table.Properties.OfType<UniqueConstraint>().ToList();
                foreach (var uniqueConstraint in uniqueConstraints)
                    GenerateUniqueConstraint(sb, uniqueConstraint);
            }

            var customProperties = table.Properties.OfType<SqlTableCustomProperty>().ToList();
            foreach (var customProperty in customProperties)
                GenerateCustomTableProperties(sb, customProperty);
        }

        protected abstract void GenerateIndex(StringBuilder sb, DataDefinition.Base.Index index);
        protected abstract void GenerateUniqueConstraint(StringBuilder sb, UniqueConstraint uniqueConstraint);

        protected abstract void GenerateCustomTableProperties(StringBuilder sb, SqlTableCustomProperty customProperty);
    }
}
