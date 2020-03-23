namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using FizzCode.DbTools.Common.Logger;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;

    public class CSharpGenerator : DocumenterBase
    {
        private readonly string _namespace;
        private readonly AbstractCSharpWriter _writer;

        public string[] AdditionalNamespaces { get; set; }
        public Func<SqlTable, string> OnTableComment { get; set; }
        public Func<SqlTable, string> OnTableAnnotation { get; set; }
        public Func<SqlColumn, string> OnColumnAnnotation { get; set; }
        public Func<SqlColumn, string> OnColumnComment { get; set; }

        public CSharpGenerator(AbstractCSharpWriter writer, SqlEngineVersion version, string databaseName, string @namespace)
            : base(writer.Context, version, databaseName)
        {
            _namespace = @namespace;
            _writer = writer;
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
            Log(LogSeverity.Information, "Starting on {DatabaseName}.", "CsGenerator", DatabaseName);

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

            Context.Logger.Log(LogSeverity.Information, "Writing Document file {FileName}", "Documenter", fileName);

            File.WriteAllText(fileName, sb.ToString(), Encoding.UTF8);
        }

        private void WritePartialMainClassHeader(StringBuilder sb)
        {
            sb.Append("namespace ")
            .AppendLine(_namespace)
            .AppendLine("{")
            .AppendLine(1, "using FizzCode.DbTools.DataDefinition;")
            .AppendLine(1, "using " + _writer.GetSqlTypeNamespace() + ";");

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

        private void WriteSingleFileHeader(StringBuilder sb)
        {
            sb.Append("namespace ")
            .AppendLine(_namespace)
            .AppendLine("{")
            //.AppendLine(1, "using FizzCode.DbTools.Configuration;")
            .AppendLine(1, "using FizzCode.DbTools.DataDefinition;")
            .AppendLine(1, "using " + _writer.GetSqlTypeNamespace() + ";");

            if (AdditionalNamespaces != null)
            {
                foreach (var line in AdditionalNamespaces)
                {
                    sb.Append(1, "using ").Append(line).AppendLine(";");
                }
            }

            sb.AppendLine()
            .Append(1, "public class ")
            .Append(DatabaseName)
            .AppendLine(" : DatabaseDeclaration")
            .AppendLine(1, "{")
            .AppendLine(2, "public " + DatabaseName + "(string defaultSchema = null, NamingStrategies namingStrategies = null)")
            .Append(3, ": base(")
            .Append("new ")
            .Append(string.Equals(_writer.TypeMapperType.Namespace, _writer.GetSqlTypeNamespace(), StringComparison.InvariantCultureIgnoreCase)
                ? _writer.TypeMapperType.Name
                : _writer.TypeMapperType.AssemblyQualifiedName)
            .AppendLine("(), null, defaultSchema, namingStrategies)")
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

        protected void GenerateTable(StringBuilder sb, SqlTable table)
        {
            var pks = table.Properties.OfType<PrimaryKey>().ToList();
            if (pks.Count == 0)
            {
                sb.AppendLine(2, "// no primary key");
            }

            var tableComment = OnTableComment?.Invoke(table);
            if (!string.IsNullOrEmpty(tableComment))
            {
                sb.Append(2, "// ").AppendLine(tableComment);
            }

            sb
                .Append(2, "public SqlTable ")
                .Append(Helper.GetSimplifiedSchemaAndTableName(table.SchemaAndTableName, DatabaseDeclaration.SchemaTableNameSeparator.ToString(CultureInfo.InvariantCulture)))
                .AppendLine(" { get; } = AddTable(table =>")
                .AppendLine(2, "{");

            var tableAnnotation = OnTableAnnotation?.Invoke(table);
            if (!string.IsNullOrEmpty(tableAnnotation))
            {
                sb.Append(3, "table").Append(tableAnnotation).AppendLine(";");
            }

            var pkColumns = table.Columns
                .Where(column => column.Table.Properties.OfType<PrimaryKey>().Any(x => x.SqlColumns.Any(y => y.SqlColumn == column)))
                .ToList();

            foreach (var column in pkColumns)
            {
                var columnAnnotation = OnColumnAnnotation?.Invoke(column);
                var columnComment = OnColumnComment?.Invoke(column);

                var columnCreation = _writer.GetColumnCreation(column, Helper, columnAnnotation, columnComment);
                sb.AppendLine(columnCreation);
            }

            var regularColumns = table.Columns
                .Where(x => !pkColumns.Contains(x))
                .ToList();

            foreach (var column in regularColumns)
            {
                var columnAnnotation = OnColumnAnnotation?.Invoke(column);
                var columnComment = OnColumnComment?.Invoke(column);

                var columnCreation = _writer.GetColumnCreation(column, Helper, columnAnnotation, columnComment);
                sb.AppendLine(columnCreation);
            }

            GenerateTableProperties(sb, table);
            sb.AppendLine(2, "});");
        }

        protected void GenerateTableProperties(StringBuilder sb, SqlTable table)
        {
            var indexes = table.Properties.OfType<DataDefinition.Index>().ToList();

            if (!Context.DocumenterSettings.NoIndexes)
            {
                foreach (var index in indexes)
                    GenerateIndex(sb, index);
            }

            var uniqueConstraints = table.Properties.OfType<UniqueConstraint>().ToList();

            foreach (var uniqueConstraint in uniqueConstraints)
                GenerateUniqueConstraint(sb, uniqueConstraint);
        }

#pragma warning disable CA1308 // Normalize strings to uppercase
        private static void GenerateIndex(StringBuilder sb, DataDefinition.Index index)
        {
            // TODO clustered

            sb.Append(3, "table.AddIndexWithName(");
            if (index.Includes.Count == 0)
            {
                sb.Append(index.Clustered.ToString().ToLowerInvariant())
                    .Append(", ");

                sb.Append("\"")
                    .Append(index.Name)
                    .Append("\", ");

                sb.Append(string.Join(", ", index.SqlColumns.Select(i => "\"" + i.SqlColumn.Name + "\"").ToList()));
            }
            else
            {
                sb.Append(index.Clustered.ToString().ToLowerInvariant())
                    .Append(", ");

                sb.Append("\"")
                    .Append(index.Name)
                    .Append("\", ");

                sb.Append("new [] {")
                    .Append(string.Join(", ", index.SqlColumns.Select(i => "\"" + i.SqlColumn.Name + "\"").ToList()))
                    .Append("}, ")
                    .Append("new [] {")
                    .Append(string.Join(", ", index.Includes.Select(i => "\"" + i.Name + "\"").ToList()))
                    .Append("}");
            }

            sb.AppendLine(");");
        }
#pragma warning restore CA1308 // Normalize strings to uppercase

        private static void GenerateUniqueConstraint(StringBuilder sb, UniqueConstraint uniqueConstraint)
        {
            sb.Append(3, "table.AddUniqueConstraintWithName(");

            sb.Append("\"");
            sb.Append(uniqueConstraint.Name);
            sb.Append("\", ");

            sb.Append(string.Join(", ", uniqueConstraint.SqlColumns.Select(c => "\"" + c.SqlColumn.Name + "\"").ToList()));

            sb.AppendLine(");");
        }
    }
}
