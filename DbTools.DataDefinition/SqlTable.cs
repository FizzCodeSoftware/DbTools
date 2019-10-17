namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;
    using System.Diagnostics;

    [DebuggerDisplay("{ToString(),nq}")]
    public class SqlTable
    {
        public DatabaseDefinition DatabaseDefinition { get; set; }
        public SchemaAndTableName SchemaAndTableName { get; set; }

        public SqlTable()
        {
        }

        public SqlTable(string schema, string name)
        {
            SchemaAndTableName = new SchemaAndTableName(schema, name);
        }

        public SqlTable(string tableName)
        {
            SchemaAndTableName = new SchemaAndTableName(tableName);
        }

        public SqlTable(SchemaAndTableName schemaAndTableName)
        {
            SchemaAndTableName = schemaAndTableName;
        }

        public override string ToString()
        {
            return SchemaAndTableName?.SchemaAndName ?? "";
        }

        public List<SqlTableProperty> Properties { get; } = new List<SqlTableProperty>();
        public ColumnsOrdered Columns { get; } = new ColumnsOrdered();

        public SqlColumn this[string columnName] => Columns[columnName];
    }
}
