namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Text;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;

    public class CsGenerator : DocumenterBase
    {
        private readonly string _namespace;

        public CsGenerator(Settings settings, string databaseName, string @namespace, ITableCustomizer tableCustomizer = null) : base(settings, databaseName, tableCustomizer)
        {
            _namespace = @namespace;
        }

        private readonly List<KeyValuePair<string, SqlTable>> _sqlTablesByCategory = new List<KeyValuePair<string, SqlTable>>();
        private readonly List<KeyValuePair<string, SqlTable>> _skippedSqlTablesByCategory = new List<KeyValuePair<string, SqlTable>>();

        public void Generate(DatabaseDefinition databaseDefinition)
        {
            GenerateMainFile();

            foreach (var table in databaseDefinition.GetTables())
            {
                if (!_tableCustomizer.ShouldSkip(table.SchemaAndTableName))
                    _sqlTablesByCategory.Add(new KeyValuePair<string, SqlTable>(_tableCustomizer.Category(table.SchemaAndTableName), table));
                else
                    _skippedSqlTablesByCategory.Add(new KeyValuePair<string, SqlTable>(_tableCustomizer.Category(table.SchemaAndTableName), table));
            }

            foreach (var tableKvp in _sqlTablesByCategory.OrderBy(kvp => kvp.Key).ThenBy(t => t.Value.SchemaAndTableName))
            {
                var category = tableKvp.Key;
                var table = tableKvp.Value;
                GenerateTable(category, table);
            }
        }

        public void GenerateMainFile()
        {
            var sb = new StringBuilder();
            sb.Append("namespace ")
                .AppendLine(_namespace)
                .AppendLine("{")
                .AppendLine(1, "using FizzCode.DbTools.DataDefinition;")
                .AppendLine();

            sb.Append(1, "public partial class ").Append(_databaseName).AppendLine(" : DatabaseDeclaration");
            sb.AppendLine(1, "{");

            sb.AppendLine(1, "}");
            sb.AppendLine("}");

            var path = ConfigurationManager.AppSettings["WorkingDirectory"]
                + _databaseName + "/" + _databaseName + ".cs";

            var fileInfo = new FileInfo(path);
            fileInfo.Directory.Create();
            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        }

        protected void GenerateTable(string category, SqlTable table)
        {
            var sb = new StringBuilder();
            sb.Append("namespace ").AppendLine(_namespace)
                .AppendLine("{")
                .AppendLine(1, "using FizzCode.DbTools.DataDefinition;")
                .AppendLine();

            sb.Append(1, "public partial class ").AppendLine(_databaseName)
                .AppendLine(1, "{");

            var pks = table.Properties.OfType<PrimaryKey>().ToList();
            if (pks.Count == 0)
            {
                sb.AppendLine(2, "// no primary key");
            }

            sb.Append("\t\tpublic static LazySqlTable ").Append(Helper.GetSimplifiedSchemaAndTableName(table.SchemaAndTableName, DatabaseDeclaration.SchemaTableNameSeparator.ToString())).AppendLine(" = new LazySqlTable(() =>")
                .AppendLine(2, "{")
                .AppendLine(3, "var table = new SqlTable();");

            var pkColumns = table.Columns.Values
                .Where(column => column.Table.Properties.OfType<PrimaryKey>().Any(x => x.SqlColumns.Any(y => y.SqlColumn == column)))
                .ToList();

            foreach (var column in pkColumns)
            {
                var line = ColumnCreationHelper.GetColumnCreation(column);
                sb.Append(line);
                sb.AppendLine();
            }

            var regularColumns = table.Columns.Values
                .Where(x => !pkColumns.Contains(x)).ToList();

            foreach (var column in regularColumns)
            {
                var line = ColumnCreationHelper.GetColumnCreation(column);
                sb.Append(line);
                sb.AppendLine();
            }

            // TODO Indexes + config

            sb.AppendLine(3, "return table;");
            sb.AppendLine(2, "});");
            sb.AppendLine(1, "}");
            sb.AppendLine("}");

            // TODO handle illegal chars
            var categoryInPath = category;
            /*if (categoryInPath == "?")
                categoryInPath = "QuestionMark";*/

            if (string.IsNullOrEmpty(categoryInPath))
                categoryInPath = "_no_category_";

            categoryInPath = categoryInPath.Replace('?', '？');

            var path = ConfigurationManager.AppSettings["WorkingDirectory"]
                + _databaseName + "/" + categoryInPath + "/" + Helper.GetSimplifiedSchemaAndTableName(table.SchemaAndTableName, ".") + ".cs";

            var fileInfo = new FileInfo(path);
            fileInfo.Directory.Create();
            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        }
    }
}
