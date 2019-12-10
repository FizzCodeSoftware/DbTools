namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using FizzCode.DbTools.Common.Logger;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionGenerator;

    public class Documenter : DocumenterBase
    {
        protected IDocumenterWriter DocumenterWriter { get; }
        protected ISqlTypeMapper SqlTypeMapper { get; } = new GenericSqlTypeMapper();

        private readonly string _fileName;
        private readonly HashSet<DocumenterFlag> _flags;

        public Documenter(DocumenterContext context, string databaseName = "", string fileName = null, HashSet<DocumenterFlag> flags = null)
            : this(new DocumenterWriterExcel(), context, databaseName, fileName, flags)
        {
        }

        public Documenter(IDocumenterWriter documenterWriter, DocumenterContext context, string databaseName = "", string fileName = null, HashSet<DocumenterFlag> flags = null)
            : base(context, databaseName)
        {
            DocumenterWriter = documenterWriter;
            _fileName = fileName;

            Helper = new DocumenterHelper(context.Settings);

            _flags = flags ?? new HashSet<DocumenterFlag>();
        }

        private readonly List<KeyValuePair<string, SqlTable>> _sqlTablesByCategory = new List<KeyValuePair<string, SqlTable>>();
        private readonly List<KeyValuePair<string, SqlTable>> _skippedSqlTablesByCategory = new List<KeyValuePair<string, SqlTable>>();

        private Color? GetColor(SchemaAndTableName schemaAndTableName)
        {
            // TODO coloring to incude schema
            var hexColor = Context.Customizer.BackGroundColor(schemaAndTableName);

            if (hexColor == null)
                return null;

            return ColorTranslator.FromHtml(hexColor);
        }

        public void Document(DatabaseDefinition databaseDefinition)
        {
            Context.Logger.Log(LogSeverity.Information, "Starting on {DatabaseName}.", "Documenter", DatabaseName);

            var tables = RemoveKnownTechnicalTables(databaseDefinition.GetTables());

            foreach (var table in tables)
            {
                if (!Context.Customizer.ShouldSkip(table.SchemaAndTableName))
                    _sqlTablesByCategory.Add(new KeyValuePair<string, SqlTable>(Context.Customizer.Category(table.SchemaAndTableName), table));
                else
                    _skippedSqlTablesByCategory.Add(new KeyValuePair<string, SqlTable>(Context.Customizer.Category(table.SchemaAndTableName), table));
            }

            var hasCategories = _sqlTablesByCategory.Any(x => !string.IsNullOrEmpty(x.Key));

            WriteLine("Database", "Database name", DatabaseName);
            WriteLine("Database", "Number of documented tables", databaseDefinition.GetTables().Count(t => !Context.Customizer.ShouldSkip(t.SchemaAndTableName)));
            WriteLine("Database", "Number of skipped tables", databaseDefinition.GetTables().Count(t => Context.Customizer.ShouldSkip(t.SchemaAndTableName)));
            WriteLine("Database", "Number of tables", databaseDefinition.GetTables().Count);

            if (hasCategories)
            {
                WriteLine("Database");
                WriteLine("Database", "Documented category", "Table count");

                Context.Logger.Log(LogSeverity.Verbose, "Writing tables by category.", "Documenter");

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

                if (!_flags.Contains(DocumenterFlag.NoInternalDataTypes))
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
                if (!_flags.Contains(DocumenterFlag.NoInternalDataTypes))
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
                Context.Logger.Log(LogSeverity.Verbose, "Generating {TableName}.", "Documenter", tableKvp.Value.SchemaAndTableName);
                var category = tableKvp.Key;
                var table = tableKvp.Value;
                AddTableToTableList(category, table, hasCategories);

                var sheetColor = GetColor(table.SchemaAndTableName);
                if (sheetColor != null)
                    DocumenterWriter.SetSheetColor(Helper.GetSimplifiedSchemaAndTableName(table.SchemaAndTableName), sheetColor.Value);

                var mergeAmount = !_flags.Contains(DocumenterFlag.NoInternalDataTypes) ? 12 : 11;

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

                if (!_flags.Contains(DocumenterFlag.NoInternalDataTypes))
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

            Context.Logger.Log(LogSeverity.Information, "Generating Document content.", "Documenter");
            var content = DocumenterWriter.GetContent();

            var fileName = _fileName ?? (DatabaseName?.Length == 0 ? "Database.xlsx" : DatabaseName + ".xlsx");

            var path = Context.DocumenterSettings?.WorkingDirectory;
            
            Context.Logger.Log(LogSeverity.Information, "Writing Document file {FileName} to folder {Folder}", "Documenter", fileName, path);

            if (!string.IsNullOrEmpty(path))
            {
                fileName = Path.Combine(path, fileName);
            }

            File.WriteAllBytes(fileName, content);
        }

        private static List<SqlTable> RemoveKnownTechnicalTables(List<SqlTable> list)
        {
            return list.Where(x => !ShouldSkipKnownTechnicalTable(x.SchemaAndTableName)).ToList();
        }

        public static bool ShouldSkipKnownTechnicalTable(SchemaAndTableName schemaAndTableName)
        {
            // TODO MS Sql specific
            // TODO Move
            // TODO Options
            return schemaAndTableName.SchemaAndName == "dbo.__RefactorLog"
                || schemaAndTableName.SchemaAndName == "dbo.sysdiagrams";
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
                var sqlType = SqlTypeMapper.GetType(column.Type);
                var descriptionProperty = column.Properties.OfType<SqlColumnDescription>().FirstOrDefault();
                var description = "";
                if (descriptionProperty != null)
                    description = descriptionProperty.Description;

                var isPk = pks.Any(pk => pk.SqlColumns.Any(cao => cao.SqlColumn == column));

                if (!_flags.Contains(DocumenterFlag.NoInternalDataTypes))
                    Write(table.SchemaAndTableName, column.Name, column.Type.ToString(), sqlType, column.Length, column.Precision, column.IsNullable);
                else
                    Write(table.SchemaAndTableName, column.Name, sqlType, column.Length, column.Precision, column.IsNullable);

                if (isPk)
                    Write(table.SchemaAndTableName, true);
                else
                    Write(table.SchemaAndTableName, "");

                var identity = column.Properties.OfType<Identity>().FirstOrDefault();

                if (identity != null)
                    Write(table.SchemaAndTableName, $"IDENTITY ({identity.Seed}, {identity.Increment})");
                else
                    Write(table.SchemaAndTableName, "");

                var defaultValue = column.Properties.OfType<DefaultValue>().FirstOrDefault();

                if (defaultValue != null)
                    Write(table.SchemaAndTableName, defaultValue);
                else
                    Write(table.SchemaAndTableName, "");

                Write(table.SchemaAndTableName, description.Trim());

                // "Foreign Key name", "Priary Key table", "Priary Key column"
                var fkOnColumn = table.Properties.OfType<ForeignKey>().FirstOrDefault(fk => fk.ForeignKeyColumns.Any(fkc => fkc.ForeignKeyColumn == column));

                if (fkOnColumn != null)
                {
                    Write(table.SchemaAndTableName, fkOnColumn.Name);
                    Write(table.SchemaAndTableName,
                        Helper.GetSimplifiedSchemaAndTableName(fkOnColumn.ReferredTable.SchemaAndTableName));
                    WriteLink(table.SchemaAndTableName, "link", fkOnColumn.ReferredTable.SchemaAndTableName);
                    Write(table.SchemaAndTableName, fkOnColumn.ForeignKeyColumns.First(fkc => fkc.ForeignKeyColumn == column).ReferredColumn.Name);
                }

                WriteLine(table.SchemaAndTableName);

                if (hasCategories)
                {
                    if (!_flags.Contains(DocumenterFlag.NoInternalDataTypes))
                        DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "All columns", category, table.SchemaAndTableName.Schema, table.SchemaAndTableName.TableName, column.Name, column.Type.ToString(), sqlType, column.Length, column.Precision, column.IsNullable);
                    else
                        DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "All columns", category, table.SchemaAndTableName.Schema, table.SchemaAndTableName.TableName, column.Name, sqlType, column.Length, column.Precision, column.IsNullable);
                }
                else if (!_flags.Contains(DocumenterFlag.NoInternalDataTypes))
                {
                    DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "All columns", table.SchemaAndTableName.Schema, table.SchemaAndTableName.TableName, column.Name, column.Type.ToString(), sqlType, column.Length, column.Precision, column.IsNullable);
                }
                else
                {
                    DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "All columns", table.SchemaAndTableName.Schema, table.SchemaAndTableName.TableName, column.Name, sqlType, column.Length, column.Precision, column.IsNullable);
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

            if (!_flags.Contains(DocumenterFlag.NoDetailedForeignKeys))
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
                        Write(table.SchemaAndTableName, fkColumn.ReferredColumn.Name);
                }

                if (fks.Count > 0)
                    WriteLine(table.SchemaAndTableName);
            }

            if (!_flags.Contains(DocumenterFlag.NoDetailedIndexes))
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
