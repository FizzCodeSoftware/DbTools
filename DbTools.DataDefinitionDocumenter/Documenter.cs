namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionGenerator;

    public class Documenter
    {
        protected IDocumenterWriter DocumenterWriter { get; }
        protected ISqlTypeMapper SqlTypeMapper { get; } = new GenericSqlTypeMapper();

        private readonly string _databaseName;
        private readonly ITableCustomizer _tableCustomizer;

        private readonly string _fileName;

        public Documenter(string databaseName = "", ITableCustomizer tableCustomizer = null, string fileName = null) : this(new DocumenterWriterExcel(), databaseName, tableCustomizer, fileName)
        {
        }

        public Documenter(IDocumenterWriter documenterWriter, string databaseName = "", ITableCustomizer tableCustomizer = null, string fileName = null)
        {
            _databaseName = databaseName;
            DocumenterWriter = documenterWriter;
            _tableCustomizer = tableCustomizer ?? new EmptyTableCustomizer();
            _fileName = fileName;
        }

        private readonly List<KeyValuePair<string, SqlTable>> _sqlTablesByCategory = new List<KeyValuePair<string, SqlTable>>();
        private readonly List<KeyValuePair<string, SqlTable>> _skippedSqlTablesByCategory = new List<KeyValuePair<string, SqlTable>>();

        private Color? GetColor(SchemaAndTableName schemaAndTableName)
        {
            // TODO coloring to incude schema
            var hexColor = _tableCustomizer.BackGroundColor(schemaAndTableName.TableName);

            if (hexColor == null)
                return null;

            return ColorTranslator.FromHtml(hexColor);
        }

        public void Document(DatabaseDefinition databaseDefinition)
        {
            DocumenterWriter.WriteLine("Database", "Database name", _databaseName);

            DocumenterWriter.WriteLine("Tables", "Category", "Schema", "Table Name", "Number of columns", "Description");

            DocumenterWriter.WriteLine("All tables", "Category", "Schema", "Table Name", "Column Name", "Data Type (DbTools)", "Data Type", "Column Length", "Column Precision", "Allow Nulls", "Primary Key", "Identity", "Default Value", "Description");

            DocumenterWriter.WriteLine("Database", "Number of documented tables", databaseDefinition.GetTables().Count(t => !_tableCustomizer.ShouldSkip(t.SchemaAndTableName.TableName)));
            DocumenterWriter.WriteLine("Database", "Number of skipped tables", databaseDefinition.GetTables().Count(t => _tableCustomizer.ShouldSkip(t.SchemaAndTableName.TableName)));
            DocumenterWriter.WriteLine("Database", "Number of tables", databaseDefinition.GetTables().Count);

            foreach (var table in databaseDefinition.GetTables())
            {
                if (!_tableCustomizer.ShouldSkip(table.SchemaAndTableName.TableName))
                    _sqlTablesByCategory.Add(new KeyValuePair<string, SqlTable>(_tableCustomizer.Category(table.SchemaAndTableName), table));
                else
                    _skippedSqlTablesByCategory.Add(new KeyValuePair<string, SqlTable>(_tableCustomizer.Category(table.SchemaAndTableName), table));
            }

            foreach (var tableKvp in _sqlTablesByCategory.OrderBy(kvp => kvp.Key).ThenBy(t => t.Value.SchemaAndTableName))
            {
                var category = tableKvp.Key;
                var table = tableKvp.Value;
                DocumenterWriter.WriteLine(GetColor(table.SchemaAndTableName), table.SchemaAndTableName, "Category", "Schema", "Table Name", "Column Name", "Data Type (DbTools)", "Data Type", "Column Length", "Column Precision", "Allow Nulls", "Primary Key", "Identity", "Default Value", "Description");
                DocumentTable(category, table);
                DocumentTableDetails(category, table);
            }

            DocumenterWriter.WriteLine("Tables");

            foreach (var tableKvp in _skippedSqlTablesByCategory.OrderBy(kvp => kvp.Key).ThenBy(t => t.Value.SchemaAndTableName))
            {
                var category = tableKvp.Key;
                var table = tableKvp.Value;
                DocumentTable(category, table);
            }

            DocumenterWriter.WriteLine("Database");

            foreach (var category in _sqlTablesByCategory.Select(kvp => kvp.Key).Distinct().OrderBy(x => x))
            {
                DocumenterWriter.WriteLine("Database", $"{category ?? "(No category)"}, number of documented tables", _sqlTablesByCategory.Count(kvp => kvp.Key == category));
            }

            DocumenterWriter.WriteLine("Database");

            foreach (var category in _skippedSqlTablesByCategory.Select(kvp => kvp.Key).Distinct().OrderBy(x => x))
            {
                DocumenterWriter.WriteLine("Database", $"{category ?? "(No category)"}, number of skipped tables", _skippedSqlTablesByCategory.Count(kvp => kvp.Key == category));
            }

            var content = DocumenterWriter.GetContent();

            var fileName = _fileName ?? (_databaseName?.Length == 0 ? "Database.xlsx" : _databaseName + ".xlsx");

            var path = ConfigurationManager.AppSettings["WorkingDirectory"];

            File.WriteAllBytes(path + fileName, content);
        }

        protected void DocumentTable(string category, SqlTable table)
        {
            DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "Tables", category);
            DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "Tables", table.SchemaAndTableName.Schema);
            DocumenterWriter.WriteLink(GetColor(table.SchemaAndTableName), "Tables", table.SchemaAndTableName.TableName);
            DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "Tables", table.Columns.Count);

            var tableDescription = table.Properties.OfType<SqlTableDescription>().FirstOrDefault();
            if(tableDescription != null)
                DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "Tables", tableDescription.Description);

            DocumenterWriter.WriteLine("Tables");
        }

        protected void DocumentTableDetails(string category, SqlTable table)
        {
            var pks = table.Properties.OfType<PrimaryKey>().ToList();

            foreach (var column in table.Columns)
            {
                // TODO Type as ISqlTypeMapper
                var sqlType = SqlTypeMapper.GetType(column.Value.Type);
                var descriptionProperty = column.Value.Properties.OfType<SqlColumnDescription>().FirstOrDefault();
                var description = "";
                if (descriptionProperty != null)
                    description = descriptionProperty.Description;

                var isPk = pks.Any(pk => pk.SqlColumns.Any(cao => cao.SqlColumn == column.Value));

                DocumenterWriter.Write(GetColor(table.SchemaAndTableName), table.SchemaAndTableName, category, table.SchemaAndTableName.Schema, table.SchemaAndTableName.TableName, column.Value.Name, column.Value.Type.ToString(), sqlType, column.Value.Length, column.Value.Precision, column.Value.IsNullable);

                if (isPk)
                    DocumenterWriter.Write(GetColor(table.SchemaAndTableName), table.SchemaAndTableName, true);
                else
                    DocumenterWriter.Write(GetColor(table.SchemaAndTableName), table.SchemaAndTableName, "");

                var identity = column.Value.Properties.OfType<Identity>().FirstOrDefault();

                if(identity != null)
                    DocumenterWriter.Write(GetColor(table.SchemaAndTableName), table.SchemaAndTableName, $"IDENTITY ({identity.Seed}, {identity.Increment})");
                else
                    DocumenterWriter.Write(GetColor(table.SchemaAndTableName), table.SchemaAndTableName, "");

                var defaultValue = column.Value.Properties.OfType<DefaultValue>().FirstOrDefault();

                if (defaultValue != null)
                    DocumenterWriter.Write(GetColor(table.SchemaAndTableName), table.SchemaAndTableName, defaultValue);
                else
                    DocumenterWriter.Write(GetColor(table.SchemaAndTableName), table.SchemaAndTableName, "");

                DocumenterWriter.WriteLine(GetColor(table.SchemaAndTableName), table.SchemaAndTableName, description.Trim());

                DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "All tables", category, table.SchemaAndTableName.Schema, table.SchemaAndTableName.TableName, column.Value.Name, column.Value.Type.ToString(), sqlType, column.Value.Length, column.Value.Precision, column.Value.IsNullable);

                if (isPk)
                    DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "All tables", true);
                else
                    DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "All tables", "");

                if (identity != null)
                    DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "All tables", $"IDENTITY ({identity.Seed}, {identity.Increment})");
                else
                    DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "All tables", "");

                DocumenterWriter.WriteLine(GetColor(table.SchemaAndTableName), "All tables", defaultValue);
                DocumenterWriter.WriteLine(GetColor(table.SchemaAndTableName), "All tables", description);
            }

            DocumenterWriter.WriteLine(table.SchemaAndTableName);
            DocumenterWriter.WriteLine(table.SchemaAndTableName, "Foreign keys");

            foreach (var fk in table.Properties.OfType<ForeignKey>())
            {
                DocumenterWriter.Write(table.SchemaAndTableName, fk.Name);
                foreach (var fkColumn in fk.ForeignKeyColumns)
                    DocumenterWriter.Write(table.SchemaAndTableName, fkColumn.ForeignKeyColumn.Name);

                DocumenterWriter.Write(table.SchemaAndTableName, "");
                DocumenterWriter.Write(table.SchemaAndTableName, fk.PrimaryKey.SqlTable.SchemaAndTableName);
                DocumenterWriter.Write(table.SchemaAndTableName, "");

                foreach (var fkColumn in fk.ForeignKeyColumns)
                    DocumenterWriter.Write(table.SchemaAndTableName, fkColumn.PrimaryKeyColumn.Name);
            }

            DocumenterWriter.WriteLine(table.SchemaAndTableName);
            DocumenterWriter.WriteLine(table.SchemaAndTableName, "Indexes");

            foreach (var index in table.Properties.OfType<Index>())
            {
                DocumenterWriter.Write(table.SchemaAndTableName, index.Name);
                foreach (var indexColumn in index.SqlColumns)
                {
                    DocumenterWriter.Write(table.SchemaAndTableName, indexColumn.SqlColumn.Name);
                    DocumenterWriter.Write(table.SchemaAndTableName, indexColumn);
                }

                DocumenterWriter.Write(table.SchemaAndTableName, "");
                DocumenterWriter.Write(table.SchemaAndTableName, "Includes:");
                foreach (var includeColumn in index.Includes)
                    DocumenterWriter.Write(table.SchemaAndTableName, includeColumn.Name);
            }
        }
    }
}
