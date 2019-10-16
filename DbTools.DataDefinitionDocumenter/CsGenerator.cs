namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;

    public class CsGenerator : DocumenterBase
    {
        private readonly string _namespace;

        public CsGenerator(DocumenterSettings documenterSettings, Settings settings, string databaseName, string @namespace, ITableCustomizer tableCustomizer = null) : base(documenterSettings, settings, databaseName, tableCustomizer)
        {
            _namespace = @namespace;
        }

        public void GenerateMultiFile(DatabaseDefinition databaseDefinition)
        {
            var sb = new StringBuilder();
            WritePartialMainClassHeader(sb);
            sb.AppendLine("}");

            var folder = Path.Combine(DocumenterSettings.WorkingDirectory ?? @".\", _databaseName);
            Directory.CreateDirectory(folder);
            File.WriteAllText(Path.Combine(folder, _databaseName + ".cs"), sb.ToString(), Encoding.UTF8);

            var sqlTablesByCategory = new List<KeyValuePair<string, SqlTable>>();
            foreach (var table in databaseDefinition.GetTables())
            {
                if (!_tableCustomizer.ShouldSkip(table.SchemaAndTableName))
                {
                    sqlTablesByCategory.Add(new KeyValuePair<string, SqlTable>(_tableCustomizer.Category(table.SchemaAndTableName), table));
                }
            }

            var tables = sqlTablesByCategory
                .OrderBy(kvp => kvp.Key)
                .ThenBy(t => t.Value.SchemaAndTableName.SchemaAndName);

            foreach (var tableKvp in tables)
            {
                var category = tableKvp.Key;
                var table = tableKvp.Value;

                WritePartialTableFileHeader(sb);
                GenerateTable(sb, table);
                WritePartialTableFileFooter(sb);

                var categoryInPath = category;
                if (string.IsNullOrEmpty(categoryInPath))
                    categoryInPath = "_no_category_";

                categoryInPath = categoryInPath.Replace('?', '？');

                folder = Path.Combine(DocumenterSettings.WorkingDirectory ?? @".\", _databaseName, categoryInPath);
                Directory.CreateDirectory(folder);
                File.WriteAllText(Path.Combine(folder, Helper.GetSimplifiedSchemaAndTableName(table.SchemaAndTableName, ".") + ".cs"), sb.ToString(), Encoding.UTF8);
            }
        }

        public void GenerateSingleFile(DatabaseDefinition databaseDefinition, string fileName)
        {
            var sb = new StringBuilder();
            WriteSingleFileHeader(sb);

            var tables = databaseDefinition.GetTables().Where(x => !_tableCustomizer.ShouldSkip(x.SchemaAndTableName));

            var index = 0;
            foreach (var table in tables)
            {
                if (index > 0)
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
            .AppendLine("\tusing FizzCode.DbTools.DataDefinition;")
            .AppendLine()
            .Append(1, "public partial class ")
            .Append(_databaseName)
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
            .Append(_databaseName)
            .AppendLine(" : DatabaseDeclaration")
            .AppendLine(1, "{");
        }

        private void WriteSingleFileFooter(StringBuilder sb)
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
                .Append(1, "public partial class ").AppendLine(_databaseName)
                .AppendLine(1, "{");
        }

        private void WritePartialTableFileFooter(StringBuilder sb)
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
                .Append(Helper.GetSimplifiedSchemaAndTableName(table.SchemaAndTableName, DatabaseDeclaration.SchemaTableNameSeparator.ToString()))
                .AppendLine(" { get; } = AddTable((table) =>")
                .AppendLine(2, "{");

            var pkColumns = table.Columns.Values
                .Where(column => column.Table.Properties.OfType<PrimaryKey>().Any(x => x.SqlColumns.Any(y => y.SqlColumn == column)))
                .ToList();

            foreach (var column in pkColumns)
            {
                var columnCreation = ColumnCreationHelper.GetColumnCreation(column);
                sb.AppendLine(columnCreation);
            }

            var regularColumns = table.Columns.Values
                .Where(x => !pkColumns.Contains(x))
                .ToList();

            foreach (var column in regularColumns)
            {
                var columnCreation = ColumnCreationHelper.GetColumnCreation(column);
                sb.AppendLine(columnCreation);
            }

            // TODO Indexes + config
            sb.AppendLine(2, "});");
        }
    }
}
