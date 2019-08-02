namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System.Collections.Generic;
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

        public Documenter(string databaseName = "", ITableCustomizer tableCustomizer = null) : this(new DocumenterWriterExcel(), databaseName, tableCustomizer)
        {
        }

        public Documenter(IDocumenterWriter documenterWriter, string databaseName = "", ITableCustomizer tableCustomizer = null)
        {
            _databaseName = databaseName;
            DocumenterWriter = documenterWriter;
            _tableCustomizer = tableCustomizer ?? new EmptyTableCustomizer();
        }

        private readonly List<KeyValuePair<string, SqlTable>> _sqlTablesByCategory = new List<KeyValuePair<string, SqlTable>>();

        private Color? GetColor(string tableName)
        {
            var hexColor = _tableCustomizer.BackGroundColor(tableName);

            if (hexColor == null)
                return null;

            return ColorTranslator.FromHtml(hexColor);
        }

        public void Document(DatabaseDefinition databaseDefinition)
        {
            DocumenterWriter.WriteLine("Database", "Database name", _databaseName);

            DocumenterWriter.WriteLine("Tables", "Category", "Table Name", "Number of columns");

            DocumenterWriter.WriteLine("All tables", "Category", "Table Name", "Column Name", "Data Type (DbTools)", "Data Type", "Column Length", "Column Precision", "Allow Nulls", "Primary Key", "Identity", "Description");

            DocumenterWriter.WriteLine("Database", "Number of tables", databaseDefinition.GetTables().Count);

            foreach (var table in databaseDefinition.GetTables())
            {
                if (!_tableCustomizer.ShouldSkip(table.Name))
                    _sqlTablesByCategory.Add(new KeyValuePair<string, SqlTable>(_tableCustomizer.Category(table.Name), table));
            }

            foreach (var tableKvp in _sqlTablesByCategory.OrderBy(kvp => kvp.Key))
            {
                var category = tableKvp.Key;
                var table = tableKvp.Value;
                DocumenterWriter.WriteLine(GetColor(table.Name), table.Name, "Category", "Table Name", "Column Name", "Data Type (DbTools)", "Data Type", "Column Length", "Column Precision", "Allow Nulls", "Primary Key", "Identity", "Description");
                DocumentTable(category, table);
            }

            var content = DocumenterWriter.GetContent();
            var fileName = _databaseName?.Length == 0 ? "Database.xlsx" : _databaseName + ".xlsx";
            File.WriteAllBytes(fileName, content);
        }

        protected void DocumentTable(string category, SqlTable table)
        {
            DocumenterWriter.WriteLine("Tables", category, table.Name, table.Columns.Count);

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

                DocumenterWriter.Write(GetColor(table.Name), table.Name, category, table.Name, column.Value.Name, column.Value.Type.ToString(), sqlType, column.Value.Length, column.Value.Precision, column.Value.IsNullable);

                if (isPk)
                    DocumenterWriter.Write(GetColor(table.Name), table.Name, true);
                else
                    DocumenterWriter.Write(GetColor(table.Name), table.Name, "");

                var identity = column.Value.Properties.OfType<Identity>().FirstOrDefault();

                if(identity != null)
                    DocumenterWriter.Write(GetColor(table.Name), table.Name, $"IDENTITY ({identity.Seed}, {identity.Increment})");
                else
                    DocumenterWriter.Write(GetColor(table.Name), table.Name, "");

                DocumenterWriter.WriteLine(GetColor(table.Name), table.Name, description.Trim());

                DocumenterWriter.Write(GetColor(table.Name), "All tables", category, table.Name, column.Value.Name, column.Value.Type.ToString(), sqlType, column.Value.Length, column.Value.Precision, column.Value.IsNullable);

                if (isPk)
                    DocumenterWriter.Write(GetColor(table.Name), "All tables", true);
                else
                    DocumenterWriter.Write(GetColor(table.Name), "All tables", "");

                if (identity != null)
                    DocumenterWriter.Write(GetColor(table.Name), "All tables", $"IDENTITY ({identity.Seed}, {identity.Increment})");
                else
                    DocumenterWriter.Write(GetColor(table.Name), "All tables", "");

                DocumenterWriter.WriteLine(GetColor(table.Name), "All tables", description);
            }

            DocumenterWriter.WriteLine(table.Name);
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
            }
        }
    }
}
