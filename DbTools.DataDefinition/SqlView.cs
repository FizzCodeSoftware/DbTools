namespace FizzCode.DbTools.DataDefinition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SqlView : SqlTableOrView
    {
        public List<SqlTableOrViewPropertyBase<SqlView>> Properties { get; } = new();

        public SqlView()
        {
        }

        public SqlView(string schema, string tableName)
            : base(schema, tableName)
        {
        }

        public SqlView(string tableName)
            : base(tableName)
        {
        }

        public SqlView(SchemaAndTableName schemaAndTableName)
            : base(schemaAndTableName)
        {
        }

        public string SqlStatementBody { get; set; }

        protected static SqlViewColumn AddColumn(Action<SqlViewColumn> configurator)
        {
            var sqlColumn = new SqlViewColumn();
            configurator.Invoke(sqlColumn);
            return sqlColumn;
        }

        public bool HasProperty<TProperty>()
            where TProperty : SqlTableOrViewPropertyBase<SqlView>
        {
            return Properties.Any(x => x is TProperty);
        }

        public SqlViewColumn this[string columnName] => Columns[columnName];
        public ViewColumnsOrdered Columns { get; } = new();
    }
}
