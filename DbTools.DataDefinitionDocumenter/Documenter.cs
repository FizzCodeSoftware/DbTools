namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using FizzCode.DbTools.Common.Logger;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;

    public class Documenter : DocumenterWriterBase
    {
        public Documenter(DocumenterContext context, SqlEngineVersion version, string databaseName = "", string fileName = null)
            : this(new DocumenterWriterExcel(), context, version, databaseName, fileName)
        {
        }

        public Documenter(IDocumenterWriter documenterWriter, DocumenterContext context, SqlEngineVersion version, string databaseName = "", string fileName = null)
            : base(documenterWriter, context, version, databaseName, fileName)
        {
            Customizer = context.Customizer is PatternMatchingTableCustomizer customizer
                ? new PatternMatchingTableCustomizerWithTablesAndItems(customizer)
                : context.Customizer;
        }

        private readonly List<KeyValuePair<string, SqlTable>> _sqlTablesByCategory = new List<KeyValuePair<string, SqlTable>>();
        private readonly List<KeyValuePair<string, SqlTable>> _skippedSqlTablesByCategory = new List<KeyValuePair<string, SqlTable>>();

        private ITableCustomizer Customizer { get; }
        public void Document(DatabaseDefinition databaseDefinition)
        {
            Log(LogSeverity.Information, "Starting on {DatabaseName}.", "Documenter", DatabaseName);

            var tables = RemoveKnownTechnicalTables(databaseDefinition.GetTables());

            foreach (var table in tables)
            {
                if (!Customizer.ShouldSkip(table.SchemaAndTableName))
                    _sqlTablesByCategory.Add(new KeyValuePair<string, SqlTable>(Customizer.Category(table.SchemaAndTableName), table));
                else
                    _skippedSqlTablesByCategory.Add(new KeyValuePair<string, SqlTable>(Customizer.Category(table.SchemaAndTableName), table));
            }

            var hasCategories = _sqlTablesByCategory.Any(x => !string.IsNullOrEmpty(x.Key));

            var noOfTables = databaseDefinition.GetTables().Count;
            var noOfNotSkippedTables = databaseDefinition.GetTables().Count(t => !Customizer.ShouldSkip(t.SchemaAndTableName));

            WriteLine("Database", "Database name", DatabaseName);
            WriteLine("Database", "Number of documented tables", noOfNotSkippedTables);
            WriteLine("Database", "Number of skipped tables", noOfTables - noOfNotSkippedTables);
            WriteLine("Database", "Number of tables", noOfTables);

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

                if (!Context.DocumenterSettings.NoInternalDataTypes)
                {
                    WriteLine("All columns", "Category", "Schema", "Table Name", "Column Name", "Data Type (DbTools)", "Data Type", "Column Length", "Column Scale", "Allow Nulls", "Primary Key", "Identity", "Default Value", "Description");
                }
                else
                {
                    WriteLine("All columns", "Category", "Schema", "Table Name", "Column Name", "Data Type", "Column Length", "Column Scale", "Allow Nulls", "Primary Key", "Identity", "Default Value", "Description");
                }
            }
            else
            {
                WriteLine("Tables", "Schema", "Table Name", "Link", "Number of columns", "Description");
                if (!Context.DocumenterSettings.NoInternalDataTypes)
                {
                    WriteLine("All columns", "Schema", "Table Name", "Column Name", "Data Type (DbTools)", "Data Type", "Column Length", "Column Scale", "Allow Nulls", "Primary Key", "Identity", "Default Value", "Description");
                }
                else
                {
                    WriteLine("All columns", "Schema", "Table Name", "Column Name", "Data Type", "Column Length", "Column Scale", "Allow Nulls", "Primary Key", "Identity", "Default Value", "Description");
                }
            }

            // Ensure sheet order
            if (Customizer is PatternMatchingTableCustomizer)
            {
                Write("Patt.ma.-tables");
                Write("Patt.ma.-patterns");
                Write("Patt.ma.-ma.s w exceptions");
                Write("Patt.ma.-no matches (unused)");
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

                AddTableHeader(hasCategories, category, table);

                AddTableDetails(category, table, hasCategories);
            }

            WriteLine("Tables");

            foreach (var tableKvp in _skippedSqlTablesByCategory.OrderBy(kvp => kvp.Key).ThenBy(t => t.Value.SchemaAndTableName.Schema).ThenBy(t => t.Value.SchemaAndTableName.TableName))
            {
                var category = tableKvp.Key;
                var table = tableKvp.Value;
                AddTableToTableList(category, table, hasCategories);
            }

            Context.Logger.Log(LogSeverity.Verbose, "Generating pattern matching info.", "Documenter");
            AddPatternMatching();
            AddPatternMatchingNoMatch();

            Log(LogSeverity.Information, "Generating Document content.", "Documenter");
            var content = DocumenterWriter.GetContent();

            var fileName = FileName ?? (DatabaseName?.Length == 0 ? "Database.xlsx" : DatabaseName + ".xlsx");

            var path = Context.DocumenterSettings?.WorkingDirectory;

            Log(LogSeverity.Information, "Writing Document file {FileName} to folder {Folder}", "Documenter", fileName, path);

            if (!string.IsNullOrEmpty(path))
            {
                fileName = Path.Combine(path, fileName);
            }

            File.WriteAllBytes(fileName, content);
        }

        private void AddPatternMatching()
        {
            if (!(Customizer is PatternMatchingTableCustomizerWithTablesAndItems))
                return;

            var customizer = (PatternMatchingTableCustomizerWithTablesAndItems)Customizer;

            WriteLine("Patt.ma.-tables", "Table schema", "Table name", "Pattern", "Pattern except", "Should skip if match", "Category if match", "Background color if match");
            foreach (var tableAndPattern in customizer.TableMatches)
            {
                var schemaAndTableName = tableAndPattern.Key;
                var items = tableAndPattern.Value;

                foreach (var item in items)
                {
                    WriteLine("Patt.ma.-tables", schemaAndTableName.Schema, schemaAndTableName.TableName, item.Pattern.ToString(), item.PatternExcept.ToString(), item.ShouldSkipIfMatch, item.CategoryIfMatch, item.BackGroundColorIfMatch);
                }
            }

            WriteLine("Patt.ma.-patterns", "Pattern", "Pattern except", "Table schema", "Table name", "Should skip if match", "Category if match", "Background color if match");
            foreach (var patternAndTable in customizer.MatchTables)
            {
                var item = patternAndTable.Key;
                var schemaAndTableNames = patternAndTable.Value;

                foreach (var schemaAndTableName in schemaAndTableNames)
                {
                    WriteLine("Patt.ma.-patterns", item.Pattern.ToString(), item.PatternExcept.ToString(), schemaAndTableName.Schema, schemaAndTableName.TableName, item.ShouldSkipIfMatch, item.CategoryIfMatch, item.BackGroundColorIfMatch);
                }
            }

            WriteLine("Patt.ma.-ma.s w exceptions", "Pattern", "Pattern except", "Table schema", "Table name", "Should skip if match", "Category if match", "Background color if match");
            foreach (var patternAndTable in customizer.MatchTablesWithException)
            {
                var item = patternAndTable.Key;
                var schemaAndTableNames = patternAndTable.Value;

                foreach (var schemaAndTableName in schemaAndTableNames)
                {
                    WriteLine("Patt.ma.-ma.s w exceptions", item.Pattern.ToString(), item.PatternExcept.ToString(), schemaAndTableName.Schema, schemaAndTableName.TableName, item.ShouldSkipIfMatch, item.CategoryIfMatch, item.BackGroundColorIfMatch);
                }
            }
        }

        private void AddPatternMatchingNoMatch()
        {
            if (!(Customizer is PatternMatchingTableCustomizerWithTablesAndItems))
                return;

            var customizer = (PatternMatchingTableCustomizerWithTablesAndItems)Customizer;

            WriteLine("Patt.ma.-no matches (unused)", "Pattern", "Pattern except", "Should skip if match", "Category if match", "Background color if match");

            foreach (var item in customizer.PatternMatchingTableCustomizer.Patterns)
            {
                if (customizer.MatchTables.ContainsKey(item))
                    continue;

                if (customizer.MatchTablesWithException.ContainsKey(item))
                    continue;

                WriteLine("Patt.ma.-no matches (unused)", item.Pattern.ToString(), item.PatternExcept.ToString(), item.ShouldSkipIfMatch, item.CategoryIfMatch, item.BackGroundColorIfMatch);
            }
        }

        protected void AddTableToTableList(string category, SqlTable table, bool hasCategories)
        {
            if (hasCategories)
            {
                DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "Tables", category);
            }

            DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "Tables", table.SchemaAndTableName.Schema);
            DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "Tables", table.SchemaAndTableName.TableName);

            if (!Customizer.ShouldSkip(table.SchemaAndTableName))
                DocumenterWriter.WriteLink("Tables", "link", Helper.GetSimplifiedSchemaAndTableName(table.SchemaAndTableName), GetColor(table.SchemaAndTableName));
            else
                DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "Tables", "");

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
                var sqlType = column.Type;

                var columnDocumentInfo = GetColumnDocumentInfo(pks, column);

                // TODO internal data types are not OK this way

                AddColumnsToTableSheet(column, columnDocumentInfo);

                if (hasCategories)
                {
                    if (!Context.DocumenterSettings.NoInternalDataTypes)
                        DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "All columns", category, table.SchemaAndTableName.Schema, table.SchemaAndTableName.TableName, column.Name, sqlType.SqlTypeInfo.SqlDataType, sqlType.SqlTypeInfo.SqlDataType, sqlType.Length, sqlType.Scale, sqlType.IsNullable);
                    else
                        DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "All columns", category, table.SchemaAndTableName.Schema, table.SchemaAndTableName.TableName, column.Name, sqlType, sqlType.Length, sqlType.Scale, sqlType.IsNullable);
                }
                else if (!Context.DocumenterSettings.NoInternalDataTypes)
                {
                    DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "All columns", table.SchemaAndTableName.Schema, table.SchemaAndTableName.TableName, column.Name, sqlType.SqlTypeInfo.SqlDataType, sqlType.SqlTypeInfo.SqlDataType, sqlType.Length, sqlType.Scale, sqlType.IsNullable);
                }
                else
                {
                    DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "All columns", table.SchemaAndTableName.Schema, table.SchemaAndTableName.TableName, column.Name, sqlType.SqlTypeInfo.SqlDataType, sqlType.Length, sqlType.Scale, sqlType.IsNullable);
                }

                if (columnDocumentInfo.IsPk)
                    DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "All columns", true);
                else
                    DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "All columns", "");

                if (columnDocumentInfo.Identity != null)
                    DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "All columns", $"IDENTITY ({columnDocumentInfo.Identity.Seed.ToString("D", CultureInfo.InvariantCulture)}, {columnDocumentInfo.Identity.Increment.ToString("D", CultureInfo.InvariantCulture)})");
                else
                    DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "All columns", "");

                DocumenterWriter.Write(GetColor(table.SchemaAndTableName), "All columns", columnDocumentInfo.DefaultValue);
                DocumenterWriter.WriteLine(GetColor(table.SchemaAndTableName), "All columns", columnDocumentInfo.Description);
            }

            AddForeignKeysToTableSheet(table);

            AdIndexesToTableSheet(table);

            AddUniqueConstraintsToTableSheet(table);
        }

        private void AddUniqueConstraintsToTableSheet(SqlTable table)
        {
            if (!Context.DocumenterSettings.NoUniqueConstraints)
            {
                WriteLine(table.SchemaAndTableName);

                var mergeAmount = 1 + (!Context.DocumenterSettings.NoInternalDataTypes ? 12 : 11);

                WriteAndMerge(table.SchemaAndTableName, mergeAmount, "Unique constraints");
                WriteLine(table.SchemaAndTableName);

                var uniqueConstraints = table.Properties.OfType<UniqueConstraint>().ToList();

                if (uniqueConstraints.Count > 0)
                    WriteLine(table.SchemaAndTableName, "Unique constraint name", "Column");

                foreach (var uniqueConstraint in uniqueConstraints)
                {
                    AddUniqueConstraint(uniqueConstraint);
                }
            }
        }

        private void AdIndexesToTableSheet(SqlTable table)
        {
            if (!Context.DocumenterSettings.NoIndexes)
            {
                WriteLine(table.SchemaAndTableName);

                var mergeAmount = 1 + (!Context.DocumenterSettings.NoInternalDataTypes ? 12 : 11);

                WriteAndMerge(table.SchemaAndTableName, mergeAmount, "Indexes");
                WriteLine(table.SchemaAndTableName);

                var indexes = table.Properties.OfType<Index>().ToList();

                if (indexes.Count > 0)
                    WriteLine(table.SchemaAndTableName, "Index name", "Column", "Order", "Include");

                foreach (var index in indexes)
                {
                    AddIndex(index);
                }
            }
        }

        private void AddForeignKeysToTableSheet(SqlTable table)
        {
            var mergeAmount = 1 + (!Context.DocumenterSettings.NoInternalDataTypes ? 12 : 11);

            if (!Context.DocumenterSettings.NoForeignKeys)
            {
                WriteLine(table.SchemaAndTableName);

                WriteAndMerge(table.SchemaAndTableName, mergeAmount, "Foreign keys");
                WriteLine(table.SchemaAndTableName);

                var fks = table.Properties.OfType<ForeignKey>().ToList();

                if (fks.Count > 0)
                {
                    // TODO allow nulls. Check / other properties?
                    WriteLine(table.SchemaAndTableName, "Foreign key name", "Column", "Referenced Table", "link", "Referenced Column", "Properties");
                }

                foreach (var fk in fks)
                {
                    AddForeignKey(fk);
                }

                if (fks.Count > 0)
                    WriteLine(table.SchemaAndTableName);
            }
        }
    }
}
