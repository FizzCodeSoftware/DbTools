namespace FizzCode.DbTools.DataDefinition
{
    using System.Diagnostics;
    using FizzCode.DbTools.DataDefinition.Base;

    [DebuggerDisplay("{ToString(),nq}")]
    public abstract class SqlTableOrView
    {
        public IDatabaseDefinition DatabaseDefinition { get; set; }
        public SchemaAndTableName SchemaAndTableName { get; set; }

        //public List<SqlTableOrViewPropertyBase<SqlTableOrView>> Properties { get; } = new();

        public SqlTableOrView()
        {
        }

        public SqlTableOrView(string schema, string tableName)
        {
            SchemaAndTableName = new SchemaAndTableName(schema, tableName);
        }

        public SqlTableOrView(string tableName)
        {
            SchemaAndTableName = new SchemaAndTableName(tableName);
        }

        public SqlTableOrView(SchemaAndTableName schemaAndTableName)
        {
            SchemaAndTableName = schemaAndTableName;
        }

        public override string ToString()
        {
            return SchemaAndTableName?.SchemaAndName ?? "";
        }
    }
}
