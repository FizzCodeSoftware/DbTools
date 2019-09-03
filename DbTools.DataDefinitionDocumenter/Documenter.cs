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
        private readonly HashSet<DocumenterFlags> _flags;

        public Documenter(string databaseName = "", ITableCustomizer tableCustomizer = null, string fileName = null, HashSet<DocumenterFlags> flags = null)
            : this(new DocumenterWriterExcel(), databaseName, tableCustomizer, fileName, flags)
        {
        }

        public Documenter(IDocumenterWriter documenterWriter, string databaseName = "", ITableCustomizer tableCustomizer = null, string fileName = null, HashSet<DocumenterFlags> flags = null)
        {
            _databaseName = databaseName;
            DocumenterWriter = documenterWriter;
            _tableCustomizer = tableCustomizer ?? new EmptyTableCustomizer();
            _fileName = fileName;

            if (flags == null)
                _flags = new HashSet<DocumenterFlags>();
            else
                _flags = flags;
        }

        private readonly List<KeyValuePair<string, SqlTable>> _sqlTablesByCategory = new List<KeyValuePair<string, SqlTable>>();
        private readonly List<KeyValuePair<string, SqlTable>> _skippedSqlTablesByCategory = new List<KeyValuePair<string, SqlTable>>();

        private Color? GetColor(SchemaAndTableName schemaAndTableName)
        {
            // TODO coloring to incude schema
            var hexColor = _tableCustomizer.BackGroundColor(schemaAndTableName);

            if (hexColor == null)
                return null;

            return ColorTranslator.FromHtml(hexColor);
        }

        public void Document(DatabaseDefinition databaseDefinition)
        {
            var tables = RemoveKnownTechnicalTables(databaseDefinition.GetTables());

            foreach (var table in tables)
            {
                if (!_tableCustomizer.ShouldSkip(table.SchemaAndTableName))
                    _sqlTablesByCategory.Add(new KeyValuePair<string, SqlTable>(_tableCustomizer.Category(table.SchemaAndTableName), table));
                else
                    _skippedSqlTablesByCategory.Add(new KeyValuePair<string, SqlTable>(_tableCustomizer.Category(table.SchemaAndTableName), table));
            }

            var hasCategories = _sqlTablesByCategory.Any(x => !string.IsNullOrEmpty(x.Key));

            if (hasCategories)
            {
                DocumenterWriter.WriteLine("Database", "Database name", _databaseName);
                DocumenterWriter.WriteLine("Database", "Number of documented tables", databaseDefinition.GetTables().Count(t => !_tableCustomizer.ShouldSkip(t.SchemaAndTableName)));
                DocumenterWriter.WriteLine("Database", "Number of skipped tables", databaseDefinition.GetTables().Count(t => _tableCustomizer.ShouldSkip(t.SchemaAndTableName)));
                DocumenterWriter.WriteLine("Database", "Number of tables", databaseDefinition.GetTables().Count);
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

                DocumenterWriter.WriteLine("Tables", "Category", "Schema", "Table Name", "Link", "Number of columns", "Description");

                if (!_flags.Contains(DocumenterFlags.NoInternalDataTypes))
                {
                    DocumenterWriter.WriteLine("All columns", "Category", "Schema", "Table Name", "Column Name", "Data Type (DbTools)", "Data Type", "Column Length", "Column Precision", "Allow Nulls", "Primary Key", "Identity", "Default Value", "Description");
                }
                else
                {
                    DocumenterWriter.WriteLine("All columns", "Category", "Schema", "Table Name", "Column Name", "Data Type", "Column Length", "Column Precision", "Allow Nulls", "Primary Key", "Identity", "Default Value", "Description");
                }
            }
            else
            {
                DocumenterWriter.WriteLine("Tables", "Schema", "Table Name", "Number of columns", "Description");
                if (!_flags.Contains(DocumenterFlags.NoInternalDataTypes))
                {
                    DocumenterWriter.WriteLine("All columns", "Schema", "Table Name", "Column Name", "Data Type (DbTools)", "Data Type", "Column Length", "Column Precision", "Allow Nulls", "Primary Key", "Identity", "Default Value", "Description");
                }
                else
                {
                    DocumenterWriter.WriteLine("All columns", "Schema", "Table Name", "Column Name", "Data Type", "Column Length", "Column Precision", "Allow Nulls", "Primary Key", "Identity", "Default Value", "Description");
                }
            }

            foreach (var tableKvp in _sqlTablesByCategory.OrderBy(kvp => kvp.Key).ThenBy(t => t.Value.SchemaAndTableName.Schema).ThenBy(t => t.Value.SchemaAndTableName.TableName))
            {
                var category = tableKvp.Key;
                var table = tableKvp.Value;
                AddTableToTableList(category, table, hasCategories);

                var sheetColor = GetColor(table.SchemaAndTableName);
                if (sheetColor != null)
                    DocumenterWriter.SetSheetColor(table.SchemaAndTableName, sheetColor.Value);

                var mergeAmount = !_flags.Contains(DocumenterFlags.NoInternalDataTypes) ? 12 : 11;

                DocumenterWriter.Write(GetColor(table.SchemaAndTableName), table.SchemaAndTableName, "Schema");
                DocumenterWriter.WriteAndMerge(GetColor(table.SchemaAndTableName), table.SchemaAndTableName, mergeAmount, table.SchemaAndTableName.Schema);
                DocumenterWriter.WriteLine(table.SchemaAndTableName);

                DocumenterWriter.Write(GetColor(table.SchemaAndTableName), table.SchemaAndTableName, "Table name");
                DocumenterWriter.WriteAndMerge(GetColor(table.SchemaAndTableName), table.SchemaAndTableName, mergeAmount, table.SchemaAndTableName.TableName);
                DocumenterWriter.WriteLine(table.SchemaAndTableName);

                var tableDescription = table.Properties.OfType<SqlTableDescription>().FirstOrDefault();
                DocumenterWriter.Write(GetColor(table.SchemaAndTableName), table.SchemaAndTableName, "Description");
                DocumenterWriter.WriteAndMerge(GetColor(table.SchemaAndTableName), table.SchemaAndTableName, mergeAmount, tableDescription?.Description);
                DocumenterWriter.WriteLine(table.SchemaAndTableName);

                if (hasCategories && !string.IsNullOrEmpty(category))
                {
                    DocumenterWriter.Write(GetColor(table.SchemaAndTableName), table.SchemaAndTableName, "Category");
                    DocumenterWriter.WriteAndMerge(GetColor(table.SchemaAndTableName), table.SchemaAndTableName, mergeAmount, category);
                    DocumenterWriter.WriteLine(table.SchemaAndTableName);
                }

                DocumenterWriter.WriteLine(table.SchemaAndTableName);

                if (!_flags.Contains(DocumenterFlags.NoInternalDataTypes))
                    DocumenterWriter.WriteLine(null, table.SchemaAndTableName, "Column Name", "Data Type (DbTools)", "Data Type", "Column Length", "Column Precision", "Allow Nulls", "Primary Key", "Identity", "Default Value", "Description", "Foreign Key Name", "Referenced Table", "Link", "Referenced Column");
                else
                    DocumenterWriter.WriteLine(null, table.SchemaAndTableName, "Column Name", "Data Type", "Column Length", "Column Precision", "Allow Nulls", "Primary Key", "Identity", "Default Value", "Description", "Foreign Key Name", "Referenced Table", "Link", "Referenced Column");

                AddTableDetails(category, table, hasCategories);
            }

            DocumenterWriter.WriteLine("Tables");

            foreach (var tableKvp in _skippedSqlTablesByCategory.OrderBy(kvp => kvp.Key).ThenBy(t => t.Value.SchemaAndTableName.Schema).ThenBy(t => t.Value.SchemaAndTableName.TableName))
            {
                var category = tableKvp.Key;
                var table = tableKvp.Value;
                AddTableToTableList(category, table, hasCategories);
            }

            var content = DocumenterWriter.GetContent();

            var fileName = _fileName ?? (_databaseName?.Length == 0 ? "Database.xlsx" : _databaseName + ".xlsx");
             
            var path = ConfigurationManager.AppSettings["WorkingDirectory"];
            path = path == null ? fileName : Path.Combine(path, fileName);

            File.WriteAllBytes(path, content);
        }

        private List<SqlTable> RemoveKnownTechnicalTables(List<SqlTable> list)
        {
            return list.Where(x =>
                x.SchemaAndTableName.SchemaAndName != "dbo.__RefactorLog"
                && x.SchemaAndTableName.SchemaAndName != "dbo.sysdiagrams"
                ).ToList();
        }

        protected void AddTableToTableList(string category, SqlTable table, bool hasCategories)
        {
            if (hasCategories)
            {
                DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "Tables", category);
            }

            DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "Tables", table.SchemaAndTableName.Schema);
            DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "Tables", table.SchemaAndTableName.TableName);
            DocumenterWriter.WriteLink("Tables", "link", table.SchemaAndTableName.SchemaAndName, GetColor(table.SchemaAndTableName));
            DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "Tables", table.Columns.Count);

            var tableDescription = table.Properties.OfType<SqlTableDescription>().FirstOrDefault();
            if (tableDescription != null)
                DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "Tables", tableDescription.Description);

            DocumenterWriter.WriteLine("Tables");
        }

        protected void AddTableDetails(string category, SqlTable table, bool hasCategories)
        {
            var pks = table.Properties.OfType<PrimaryKey>().ToList();

            foreach (var column in table.Columns)
            {
                // TODO Create ISqlTypeMapper according to SqlDialect
                var sqlType = SqlTypeMapper.GetType(column.Value.Type);
                var descriptionProperty = column.Value.Properties.OfType<SqlColumnDescription>().FirstOrDefault();
                var description = string.Empty;
                if (descriptionProperty != null)
                    description = descriptionProperty.Description;

                var isPk = pks.Any(pk => pk.SqlColumns.Any(cao => cao.SqlColumn == column.Value));

                if (!_flags.Contains(DocumenterFlags.NoInternalDataTypes))
                    DocumenterWriter.Write(null, table.SchemaAndTableName, column.Value.Name, column.Value.Type.ToString(), sqlType, column.Value.Length, column.Value.Precision, column.Value.IsNullable);
                else
                    DocumenterWriter.Write(null, table.SchemaAndTableName, column.Value.Name, sqlType, column.Value.Length, column.Value.Precision, column.Value.IsNullable);

                if (isPk)
                    DocumenterWriter.Write(null, table.SchemaAndTableName, true);
                else
                    DocumenterWriter.Write(null, table.SchemaAndTableName, "");

                var identity = column.Value.Properties.OfType<Identity>().FirstOrDefault();

                if (identity != null)
                    DocumenterWriter.Write(null, table.SchemaAndTableName, $"IDENTITY ({identity.Seed}, {identity.Increment})");
                else
                    DocumenterWriter.Write(null, table.SchemaAndTableName, "");

                var defaultValue = column.Value.Properties.OfType<DefaultValue>().FirstOrDefault();

                if (defaultValue != null)
                    DocumenterWriter.Write(null, table.SchemaAndTableName, defaultValue);
                else
                    DocumenterWriter.Write(null, table.SchemaAndTableName, "");

                DocumenterWriter.Write(null, table.SchemaAndTableName, description.Trim());

                // "Foreign Key name", "Priary Key table", "Priary Key column"
                var fkOnColumn = table.Properties.OfType<ForeignKey>().FirstOrDefault(fk => fk.ForeignKeyColumns.Any(fkc => fkc.ForeignKeyColumn == column.Value));

                if (fkOnColumn != null)
                {
                    DocumenterWriter.Write(null, table.SchemaAndTableName, fkOnColumn.Name);
                    DocumenterWriter.Write(null, table.SchemaAndTableName, fkOnColumn.PrimaryKey.SqlTable.SchemaAndTableName.SchemaAndName);
                    DocumenterWriter.WriteLink(table.SchemaAndTableName, "link", fkOnColumn.PrimaryKey.SqlTable.SchemaAndTableName.SchemaAndName);
                    DocumenterWriter.Write(null, table.SchemaAndTableName, fkOnColumn.ForeignKeyColumns.First(fkc => fkc.ForeignKeyColumn == column.Value).PrimaryKeyColumn.Name);
                }

                DocumenterWriter.WriteLine(table.SchemaAndTableName);

                if (hasCategories)
                {
                    if (!_flags.Contains(DocumenterFlags.NoInternalDataTypes))
                        DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "All columns", category, table.SchemaAndTableName.Schema, table.SchemaAndTableName.TableName, column.Value.Name, column.Value.Type.ToString(), sqlType, column.Value.Length, column.Value.Precision, column.Value.IsNullable);
                    else
                        DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "All columns", category, table.SchemaAndTableName.Schema, table.SchemaAndTableName.TableName, column.Value.Name, sqlType, column.Value.Length, column.Value.Precision, column.Value.IsNullable);
                }
                else
                {
                    if (!_flags.Contains(DocumenterFlags.NoInternalDataTypes))
                        DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "All columns", table.SchemaAndTableName.Schema, table.SchemaAndTableName.TableName, column.Value.Name, column.Value.Type.ToString(), sqlType, column.Value.Length, column.Value.Precision, column.Value.IsNullable);
                    else
                        DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "All columns", table.SchemaAndTableName.Schema, table.SchemaAndTableName.TableName, column.Value.Name, sqlType, column.Value.Length, column.Value.Precision, column.Value.IsNullable);
                }

                if (isPk)
                    DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "All columns", true);
                else
                    DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "All columns", "");

                if (identity != null)
                    DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "All columns", $"IDENTITY ({identity.Seed}, {identity.Increment})");
                else
                    DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "All columns", "");

                DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "All columns", defaultValue);
                DocumenterWriter.WriteLine(GetColor(table.SchemaAndTableName), "All columns", description);
            }

            if (!_flags.Contains(DocumenterFlags.NoDetailedForeignKeys))
            {
                DocumenterWriter.WriteLine(table.SchemaAndTableName);
                DocumenterWriter.WriteLine(table.SchemaAndTableName, "Foreign keys");

                var fks = table.Properties.OfType<ForeignKey>().ToList();
                foreach (var fk in fks)
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

                if (fks.Count > 0)
                    DocumenterWriter.WriteLine(table.SchemaAndTableName);
            }

            if (!_flags.Contains(DocumenterFlags.NoDetailedIndexes))
            {
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
}
