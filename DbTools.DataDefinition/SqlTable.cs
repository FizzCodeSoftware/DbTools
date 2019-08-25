namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;

    public class SqlTable
    {
        public SchemaAndTableName SchemaAndTableName { get; protected set; }

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
            return SchemaAndTableName;
        }

        public List<SqlTableProperty> Properties { get; } = new List<SqlTableProperty>();
        public Dictionary<string, SqlColumn> Columns { get; } = new Dictionary<string, SqlColumn>();

        public SqlColumn this[string columnName] => Columns[columnName];
    }
}
