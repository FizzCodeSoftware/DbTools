namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;

    public class SchemaAndTableName
    {
        public string Schema { get; protected set; }
        public string TableName { get; protected set; }

        public SchemaAndTableName(string name)
        {
            TableName = name;
        }

        public SchemaAndTableName(string schema, string name)
        {
            Schema = schema;
            TableName = name;
        }

        public string SchemaAndName
        {
            get
            {
                if (Schema == null)
                    return TableName;

                return Schema + "." + TableName;
            }
        }

        public override string ToString()
        {
            return SchemaAndName;
        }

        public static implicit operator string(SchemaAndTableName schemaAndTableName)
        {
            return schemaAndTableName.ToString();
        }
    }

    public class SqlTable
    {
        public SchemaAndTableName SchemaAndTableName { get; protected set; }

        public SqlTable(string schema, string name)
        {
            SchemaAndTableName = new SchemaAndTableName(schema, name);
        }

        public SqlTable(string name)
        {
            SchemaAndTableName = new SchemaAndTableName(name);
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
