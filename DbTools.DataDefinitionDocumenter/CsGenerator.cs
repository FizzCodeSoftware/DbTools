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
                .AppendLine("\tusing FizzCode.DbTools.DataDefinition;")
                .AppendLine();

            sb.Append("\tpublic partial class ").Append(_databaseName).AppendLine(" : DatabaseDeclaration");
            sb.AppendLine("\t{");

            sb.AppendLine("\t}");
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
                .AppendLine("\tusing FizzCode.DbTools.DataDefinition;")
                .AppendLine();

            sb.Append("\tpublic partial class ").AppendLine(_databaseName)
                .AppendLine("\t{");

            var pks = table.Properties.OfType<PrimaryKey>().ToList();
            if (pks.Count == 0)
            {
                sb.AppendLine("\t\t// no primary key");
            }

            // TODO
            // - format schema and table name
            // - configure use of default schema
            sb.Append("\t\tpublic SqlTable ").Append(Helper.GetSimplifiedSchemaAndTableName(table.SchemaAndTableName, DatabaseDeclaration.SchemaTableNameSeparator.ToString())).AppendLine(" {get;} = InitTable(() =>")
                .AppendLine("\t\t{")
                .AppendLine("\t\t\t");

            var pkColumns = table.Columns.Values
                .Where(column => column.Table.Properties.OfType<PrimaryKey>().Any(x => x.SqlColumns.Any(y => y.SqlColumn == column)))
                .ToList();

            foreach (var column in pkColumns)
            {
                var line = ColumnCreationHelper.GetColumnCreation(column);
                sb.Append(line);

                var descriptionProperty = column.Properties.OfType<SqlColumnDescription>().FirstOrDefault();
                if (!string.IsNullOrEmpty(descriptionProperty?.Description))
                {
                    sb.Append(" // ").Append(descriptionProperty.Description.Replace("\r", string.Empty).Replace("\n", string.Empty));
                }

                sb.AppendLine();
            }

            var regularColumns = table.Columns.Values
                .Where(x => !pkColumns.Contains(x)).ToList();

            foreach (var column in regularColumns)
            {
                // TODO Type as ISqlTypeMapper

                var line = ColumnCreationHelper.GetColumnCreation(column);
                sb.Append(line);

                var descriptionProperty = column.Properties.OfType<SqlColumnDescription>().FirstOrDefault();
                if (!string.IsNullOrEmpty(descriptionProperty?.Description))
                {
                    sb.Append(" // ").Append(descriptionProperty.Description.Replace("\r", string.Empty).Replace("\n", string.Empty));
                }

                sb.AppendLine();
                // DocumenterWriter.Write(table.Name, category, table.Name, column.Value.Name, column.Value.Type.ToString(), sqlType, column.Value.Length, column.Value.Precision, column.Value.IsNullable);

                /*if (isPk)
                    DocumenterWriter.Write(table.Name, true);
                else
                    DocumenterWriter.Write(table.Name, "");

                var identity = column.Value.Properties.OfType<Identity>().FirstOrDefault();

                if(identity != null)
                    DocumenterWriter.Write(table.Name, $"IDENTITY ({identity.Seed}, {identity.Increment})");
                else
                    DocumenterWriter.Write(table.Name, "");

                var defaultValue = column.Value.Properties.OfType<DefaultValue>().FirstOrDefault();

                if (defaultValue != null)
                    DocumenterWriter.Write(table.Name, defaultValue);
                else
                    DocumenterWriter.Write(table.Name, "");

                DocumenterWriter.WriteLine(table.Name, description.Trim());*/

            }

            /*DocumenterWriter.WriteLine(table.Name);
            DocumenterWriter.WriteLine(table.Name, "Foreign keys");

            foreach (var fk in table.Properties.OfType<ForeignKey>())
            {
                DocumenterWriter.Write(table.Name, fk.Name);
                foreach (var fkColumn in fk.ForeignKeyColumns)
                    DocumenterWriter.Write(table.Name, fkColumn.ForeignKeyColumn.Name);

                DocumenterWriter.Write(table.Name, "");
                DocumenterWriter.Write(table.Name, fk.PrimaryKey.SqlTable.Name);
                DocumenterWriter.Write(table.Name, "");

                foreach (var fkColumn in fk.ForeignKeyColumns)
                    DocumenterWriter.Write(table.Name, fkColumn.PrimaryKeyColumn.Name);
            }

            DocumenterWriter.WriteLine(table.Name);
            DocumenterWriter.WriteLine(table.Name, "Indexes");

            foreach (var index in table.Properties.OfType<Index>())
            {
                DocumenterWriter.Write(table.Name, index.Name);
                foreach (var indexColumn in index.SqlColumns)
                {
                    DocumenterWriter.Write(table.Name, indexColumn.SqlColumn.Name);
                    DocumenterWriter.Write(table.Name, indexColumn);
                }

                DocumenterWriter.Write(table.Name, "");
                DocumenterWriter.Write(table.Name, "Includes:");
                foreach (var includeColumn in index.Includes)
                    DocumenterWriter.Write(table.Name, includeColumn.Name);
            }*/

            sb.AppendLine("\t\t\t");
            sb.AppendLine("\t\t});");
            sb.AppendLine("\t}");
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
