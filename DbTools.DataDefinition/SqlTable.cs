namespace FizzCode.DbTools.DataDefinition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SqlTable : SqlTableOrView
    {
        public List<SqlTableOrViewPropertyBase<SqlTable>> Properties { get; } = new();

        public SqlTable()
        {
        }

        public SqlTable(string schema, string tableName)
            : base(schema, tableName)
        {
        }

        public SqlTable(string tableName)
            : base(tableName)
        {
        }

        public SqlTable(SchemaAndTableName schemaAndTableName)
            : base(schemaAndTableName)
        {
        }

        protected static SqlColumn AddColumn(Action<SqlColumn> configurator)
        {
            var sqlColumn = new SqlColumn();
            configurator.Invoke(sqlColumn);
            return sqlColumn;
        }

        public bool HasProperty<TProperty>()
            where TProperty : SqlTableOrViewPropertyBase<SqlTable>
        {
            return Properties.Any(x => x is TProperty);
        }

        public SqlColumn this[string columnName] => Columns[columnName];
        public ColumnsOrdered Columns { get; } = new();
    }
}
