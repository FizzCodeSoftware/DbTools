namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System.IO;
    using System.Linq;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionGenerator;

    public class Documenter
    {
        protected IDocumenterWriter DocumenterWriter { get; }
        protected ISqlTypeMapper SqlTypeMapper { get; } = new GenericSqlTypeMapper();

        private readonly string _databaseName;

        public Documenter(string databaseName = "") : this(new DocumenterWriterExcel(), databaseName)
        {
        }

        public Documenter(IDocumenterWriter documenterWriter, string databaseName = "")
        {
            _databaseName = databaseName;
            DocumenterWriter = documenterWriter;
        }

        public void Document(DatabaseDefinition databaseDefinition)
        {
            DocumenterWriter.WriteLine("Database", "Database name", _databaseName);

            DocumenterWriter.WriteLine("Tables", "Table Name", "Number of columns");

            DocumenterWriter.WriteLine("All tables", "Table Name", "Column Name", "Data Type (DbTools)", "Data Type", "Column Length", "Column Precision", "Allow Nulls", "Primary Key", "Identity", "Description");

            DocumenterWriter.WriteLine("Database", "Number of tables", databaseDefinition.GetTables().Count);

            foreach (var table in databaseDefinition.GetTables())
            {
                DocumenterWriter.WriteLine(table.Name, "Table Name", "Column Name", "Data Type (DbTools)", "Data Type", "Column Length", "Column Precision", "Allow Nulls", "Primary Key", "Identity", "Description");
                DocumentTable(table);
            }

            // Summary

            var content = DocumenterWriter.GetContent();
            var fileName = _databaseName?.Length == 0 ? "Database.xlsx" : _databaseName + ".xlsx";
            File.WriteAllBytes(fileName, content);
        }

        protected void DocumentTable(SqlTable table)
        {
            DocumenterWriter.WriteLine("Tables", table.Name, table.Columns.Count);

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

                DocumenterWriter.Write(table.Name, table.Name, column.Value.Name, column.Value.Type.ToString(), sqlType, column.Value.Length, column.Value.Precision, column.Value.IsNullable);

                if (isPk)
                    DocumenterWriter.Write(table.Name, true);
                else
                    DocumenterWriter.Write(table.Name, "");

                var identity = column.Value.Properties.OfType<Identity>().FirstOrDefault();

                if(identity != null)
                    DocumenterWriter.Write(table.Name, $"IDENTITY ({identity.Seed}, {identity.Increment})");
                else
                    DocumenterWriter.Write(table.Name, "");

                DocumenterWriter.WriteLine(table.Name, description);

                DocumenterWriter.Write("All tables", table.Name, column.Value.Name, column.Value.Type.ToString(), sqlType, column.Value.Length, column.Value.Precision, column.Value.IsNullable);

                if (isPk)
                    DocumenterWriter.Write("All tables", true);
                else
                    DocumenterWriter.Write("All tables", "");

                if (identity != null)
                    DocumenterWriter.Write("All tables", $"IDENTITY ({identity.Seed}, {identity.Increment})");
                else
                    DocumenterWriter.Write("All tables", "");

                DocumenterWriter.WriteLine("All tables", description);
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
