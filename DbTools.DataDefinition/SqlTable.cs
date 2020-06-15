namespace FizzCode.DbTools.DataDefinition
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    [DebuggerDisplay("{ToString(),nq}")]
    public class SqlTable
    {
        public DatabaseDefinition DatabaseDefinition { get; set; }
        public SchemaAndTableName SchemaAndTableName { get; set; }
        public List<SqlTableProperty> Properties { get; } = new List<SqlTableProperty>();
        public ColumnsOrdered Columns { get; } = new ColumnsOrdered();

        public SqlTable()
        {
        }

        public SqlTable(string schema, string tableName)
        {
            SchemaAndTableName = new SchemaAndTableName(schema, tableName);
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

        public bool HasProperty<T>()
            where T : SqlTableProperty
        {
            return Properties.Any(x => x is T);
        }

        public SqlColumn this[string columnName] => Columns[columnName];

        protected static SqlColumn AddColumn(Action<SqlColumn> configurator)
        {
            var sqlColumn = new SqlColumn();
            configurator.Invoke(sqlColumn);
            return sqlColumn;
        }

        public string Alias { get; set; }
    }
}
