namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionGenerator;

    public class Documenter : DocumenterBase
    {
        protected IDocumenterWriter DocumenterWriter { get; }
        protected ISqlTypeMapper SqlTypeMapper { get; } = new GenericSqlTypeMapper();

        private readonly string _fileName;
        private readonly HashSet<DocumenterFlags> _flags;

        public Documenter(Settings settings, string databaseName = "", ITableCustomizer tableCustomizer = null, string fileName = null, HashSet<DocumenterFlags> flags = null)
            : this(new DocumenterWriterExcel(), settings, databaseName, tableCustomizer, fileName, flags)
        {
        }

        public Documenter(IDocumenterWriter documenterWriter, Settings settings, string databaseName = "", ITableCustomizer tableCustomizer = null, string fileName = null, HashSet<DocumenterFlags> flags = null) : base(settings, databaseName, tableCustomizer)
        {
            DocumenterWriter = documenterWriter;
            _fileName = fileName;

            Helper = new DocumenterHelper(settings);

            _flags = flags ?? new HashSet<DocumenterFlags>();
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

            WriteLine("Database", "Database name", _databaseName);
            WriteLine("Database", "Number of documented tables", databaseDefinition.GetTables().Count(t => !_tableCustomizer.ShouldSkip(t.SchemaAndTableName)));
            WriteLine("Database", "Number of skipped tables", databaseDefinition.GetTables().Count(t => _tableCustomizer.ShouldSkip(t.SchemaAndTableName)));
            WriteLine("Database", "Number of tables", databaseDefinition.GetTables().Count);

            if (hasCategories)
            {
                WriteLine("Database");
                WriteLine("Database", "Documented category", "Table count");

                foreach (var category in _sqlTablesByCategory.Select(kvp => kvp.Key).Distinct().OrderBy(x => x))
                {
                    WriteLine("Database", category ?? "(No category)", _sqlTablesByCategory.Count(kvp => kvp.Key == category));
                }

                if (_skippedSqlTablesByCategory.Count > 0)
                {
                    WriteLine("Database");
                    WriteLine("Database", "Skipped category", "Table count");

                    foreach (var category in _skippedSqlTablesByCategory.Select(kvp => kvp.Key).Distinct().OrderBy(x => x))
                    {
                        WriteLine("Database", category ?? "(No category)", _skippedSqlTablesByCategory.Count(kvp => kvp.Key == category));
                    }
                }

                WriteLine("Tables", "Category", "Schema", "Table Name", "Link", "Number of columns", "Description");

                if (!_flags.Contains(DocumenterFlags.NoInternalDataTypes))
                {
                    WriteLine("All columns", "Category", "Schema", "Table Name", "Column Name", "Data Type (DbTools)", "Data Type", "Column Length", "Column Precision", "Allow Nulls", "Primary Key", "Identity", "Default Value", "Description");
                }
                else
                {
                    WriteLine("All columns", "Category", "Schema", "Table Name", "Column Name", "Data Type", "Column Length", "Column Precision", "Allow Nulls", "Primary Key", "Identity", "Default Value", "Description");
                }
            }
            else
            {
                WriteLine("Tables", "Schema", "Table Name", "Number of columns", "Description");
                if (!_flags.Contains(DocumenterFlags.NoInternalDataTypes))
                {
                    WriteLine("All columns", "Schema", "Table Name", "Column Name", "Data Type (DbTools)", "Data Type", "Column Length", "Column Precision", "Allow Nulls", "Primary Key", "Identity", "Default Value", "Description");
                }
                else
                {
                    WriteLine("All columns", "Schema", "Table Name", "Column Name", "Data Type", "Column Length", "Column Precision", "Allow Nulls", "Primary Key", "Identity", "Default Value", "Description");
                }
            }

            foreach (var tableKvp in _sqlTablesByCategory.OrderBy(kvp => kvp.Key).ThenBy(t => t.Value.SchemaAndTableName.Schema).ThenBy(t => t.Value.SchemaAndTableName.TableName))
            {
                var category = tableKvp.Key;
                var table = tableKvp.Value;
                AddTableToTableList(category, table, hasCategories);

                var sheetColor = GetColor(table.SchemaAndTableName);
                if (sheetColor != null)
                    DocumenterWriter.SetSheetColor(Helper.GetSimplifiedSchemaAndTableName(table.SchemaAndTableName), sheetColor.Value);

                var mergeAmount = !_flags.Contains(DocumenterFlags.NoInternalDataTypes) ? 12 : 11;

                WriteColor(table.SchemaAndTableName, "Schema");
                WriteAndMerge(table.SchemaAndTableName, mergeAmount, table.SchemaAndTableName.Schema);
                WriteLine(table.SchemaAndTableName);

                WriteColor(table.SchemaAndTableName, "Table name");
                WriteAndMerge(table.SchemaAndTableName, mergeAmount, table.SchemaAndTableName.TableName);
                WriteLine(table.SchemaAndTableName);

                var tableDescription = table.Properties.OfType<SqlTableDescription>().FirstOrDefault();
                WriteColor(table.SchemaAndTableName, "Description");
                WriteAndMerge(table.SchemaAndTableName, mergeAmount, tableDescription?.Description);
                WriteLine(table.SchemaAndTableName);

                if (hasCategories && !string.IsNullOrEmpty(category))
                {
                    WriteColor(table.SchemaAndTableName, "Category");
                    WriteAndMerge(table.SchemaAndTableName, mergeAmount, category);
                    WriteLine(table.SchemaAndTableName);
                }

                WriteLine(table.SchemaAndTableName);

                if (!_flags.Contains(DocumenterFlags.NoInternalDataTypes))
                    WriteLine(table.SchemaAndTableName, "Column Name", "Data Type (DbTools)", "Data Type", "Column Length", "Column Precision", "Allow Nulls", "Primary Key", "Identity", "Default Value", "Description", "Foreign Key Name", "Referenced Table", "Link", "Referenced Column");
                else
                    WriteLine(table.SchemaAndTableName, "Column Name", "Data Type", "Column Length", "Column Precision", "Allow Nulls", "Primary Key", "Identity", "Default Value", "Description", "Foreign Key Name", "Referenced Table", "Link", "Referenced Column");

                AddTableDetails(category, table, hasCategories);
            }

            WriteLine("Tables");

            foreach (var tableKvp in _skippedSqlTablesByCategory.OrderBy(kvp => kvp.Key).ThenBy(t => t.Value.SchemaAndTableName.Schema).ThenBy(t => t.Value.SchemaAndTableName.TableName))
            {
                var category = tableKvp.Key;
                var table = tableKvp.Value;
                AddTableToTableList(category, table, hasCategories);
            }

            var content = DocumenterWriter.GetContent();

            var fileName = _fileName ?? (_databaseName?.Length == 0 ? "Database.xlsx" : _databaseName + ".xlsx");

            var path = ConfigurationManager.AppSettings["WorkingDirectory"];
            if (!string.IsNullOrEmpty(path))
            {
                fileName = Path.Combine(path, fileName);
            }

            File.WriteAllBytes(fileName, content);
        }

        private List<SqlTable> RemoveKnownTechnicalTables(List<SqlTable> list)
        {
            // TODO MS Sql specific
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
            DocumenterWriter.WriteLink("Tables", "link", Helper.GetSimplifiedSchemaAndTableName(table.SchemaAndTableName), GetColor(table.SchemaAndTableName));
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
                    Write(table.SchemaAndTableName, column.Value.Name, column.Value.Type.ToString(), sqlType, column.Value.Length, column.Value.Precision, column.Value.IsNullable);
                else
                    Write(table.SchemaAndTableName, column.Value.Name, sqlType, column.Value.Length, column.Value.Precision, column.Value.IsNullable);

                if (isPk)
                    Write(table.SchemaAndTableName, true);
                else
                    Write(table.SchemaAndTableName, "");

                var identity = column.Value.Properties.OfType<Identity>().FirstOrDefault();

                if (identity != null)
                    Write(table.SchemaAndTableName, $"IDENTITY ({identity.Seed}, {identity.Increment})");
                else
                    Write(table.SchemaAndTableName, "");

                var defaultValue = column.Value.Properties.OfType<DefaultValue>().FirstOrDefault();

                if (defaultValue != null)
                    Write(table.SchemaAndTableName, defaultValue);
                else
                    Write(table.SchemaAndTableName, "");

                Write(table.SchemaAndTableName, description.Trim());

                // "Foreign Key name", "Priary Key table", "Priary Key column"
                var fkOnColumn = table.Properties.OfType<ForeignKey>().FirstOrDefault(fk => fk.ForeignKeyColumns.Any(fkc => fkc.ForeignKeyColumn == column.Value));

                if (fkOnColumn != null)
                {
                    Write(table.SchemaAndTableName, fkOnColumn.Name);
                    Write(table.SchemaAndTableName,
                        Helper.GetSimplifiedSchemaAndTableName(fkOnColumn.ReferredTable.SchemaAndTableName));
                    WriteLink(table.SchemaAndTableName, "link", fkOnColumn.ReferredTable.SchemaAndTableName);
                    Write(table.SchemaAndTableName, fkOnColumn.ForeignKeyColumns.First(fkc => fkc.ForeignKeyColumn == column.Value).ReferredColumnName);
                }

                WriteLine(table.SchemaAndTableName);

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
                WriteLine(table.SchemaAndTableName);
                WriteLine(table.SchemaAndTableName, "Foreign keys");

                var fks = table.Properties.OfType<ForeignKey>().ToList();
                foreach (var fk in fks)
                {
                    Write(table.SchemaAndTableName, fk.Name);
                    foreach (var fkColumn in fk.ForeignKeyColumns)
                        Write(table.SchemaAndTableName, fkColumn.ForeignKeyColumn.Name);

                    Write(table.SchemaAndTableName, "");
                    Write(table.SchemaAndTableName, Helper.GetSimplifiedSchemaAndTableName(fk.ReferredTable.SchemaAndTableName));
                    Write(table.SchemaAndTableName, "");

                    foreach (var fkColumn in fk.ForeignKeyColumns)
                        Write(table.SchemaAndTableName, fkColumn.ReferredColumnName);
                }

                if (fks.Count > 0)
                    WriteLine(table.SchemaAndTableName);
            }

            if (!_flags.Contains(DocumenterFlags.NoDetailedIndexes))
            {
                WriteLine(table.SchemaAndTableName);
                WriteLine(table.SchemaAndTableName, "Indexes");

                foreach (var index in table.Properties.OfType<Index>())
                {
                    Write(table.SchemaAndTableName, index.Name);
                    foreach (var indexColumn in index.SqlColumns)
                    {
                        Write(table.SchemaAndTableName, indexColumn.SqlColumn.Name);
                        Write(table.SchemaAndTableName, indexColumn);
                    }

                    Write(table.SchemaAndTableName, "");
                    Write(table.SchemaAndTableName, "Includes:");
                    foreach (var includeColumn in index.Includes)
                        Write(table.SchemaAndTableName, includeColumn.Name);
                }
            }
        }

        protected void Write(string sheetName, params object[] content)
        {
            DocumenterWriter.Write(sheetName, content);
        }

        protected void WriteLine(string sheetName, params object[] content)
        {
            DocumenterWriter.WriteLine(sheetName, content);
        }

        protected void Write(SchemaAndTableName schemaAndTableName, params object[] content)
        {
            DocumenterWriter.Write(Helper.GetSimplifiedSchemaAndTableName(schemaAndTableName), content);
        }

        protected void WriteColor(SchemaAndTableName schemaAndTableName, params object[] content)
        {
            DocumenterWriter.Write(GetColor(schemaAndTableName), Helper.GetSimplifiedSchemaAndTableName(schemaAndTableName), content);
        }

        protected void WriteLine(SchemaAndTableName schemaAndTableName, params object[] content)
        {
            DocumenterWriter.WriteLine(Helper.GetSimplifiedSchemaAndTableName(schemaAndTableName), content);
        }

        protected void WriteLink(SchemaAndTableName schemaAndTableName, string text, SchemaAndTableName targetSchemaAndTableName, Color? backgroundColor = null)
        {
            DocumenterWriter.WriteLink(Helper.GetSimplifiedSchemaAndTableName(schemaAndTableName), text, Helper.GetSimplifiedSchemaAndTableName(targetSchemaAndTableName), backgroundColor);
        }

        protected void WriteAndMerge(SchemaAndTableName schemaAndTableName, int mergeAmount, params object[] content)
        {
            DocumenterWriter.WriteAndMerge(GetColor(schemaAndTableName), Helper.GetSimplifiedSchemaAndTableName(schemaAndTableName), mergeAmount, content);
        }
    }
}
