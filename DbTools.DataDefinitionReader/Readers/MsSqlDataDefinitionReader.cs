namespace FizzCode.DbTools.DataDefinitionReader
{
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionExecuter;
    using FizzCode.DbTools.DataDefinitionGenerator;

    public class MsSqlDataDefinitionReader : GenericDataDefinitionReader
    {
        public MsSqlDataDefinitionReader(SqlExecuter sqlExecuter)
            : base(sqlExecuter)
        {
        }

        public override DatabaseDefinition GetDatabaseDefinition()
        {
            var dd = new DatabaseDefinition();

            foreach (var tableName in GetTableNames())
                dd.AddTable(GetTableDefinition(tableName, false));

            AddTableDocumentation(dd);

            new MsSqlIdentityReader(_executer).GetIdentity(dd);
            new MsSqlPrimaryKeyReader(_executer).GetPrimaryKey(dd);
            new MsSqlForeignKeyReader(_executer).GetForeignKeys(dd);

            return dd;
        }

        public override List<string> GetTableNames()
        {
            var reader = _executer.ExecuteQuery("SELECT name FROM sysobjects WHERE xtype = 'U'");
            return reader.GetRows<string>().ToList();
        }

        private MsSqlTableReader _tableReader;

        private MsSqlTableReader TableReader
        {
            get
            {
                return _tableReader ?? (_tableReader = new MsSqlTableReader(_executer));
            }
        }

        public override SqlTable GetTableDefinition(string tableName, bool fullDefinition = true)
        {
            var sqlTable = TableReader.GetTableDefinition(tableName);

            if (fullDefinition)
            {
                new MsSqlPrimaryKeyReader(_executer).
                GetPrimaryKey(sqlTable);
                new MsSqlForeignKeyReader(_executer).GetForeignKeys(sqlTable, null);
                AddTableDocumentation(sqlTable);
            }

            AddColumnDocumentation(sqlTable);

            return sqlTable;
        }

        public void AddColumnDocumentation(SqlTable table)
        {
            var reader = _executer.ExecuteQuery(new SqlStatementWithParameters(@"
SELECT
    c.name ColumnName,
    p.value Property
FROM
    sys.tables t
    INNER JOIN sys.all_columns c ON c.object_id = t.object_id
    INNER JOIN sys.extended_properties p ON p.major_id = t.object_id AND p.minor_id = c.column_id AND p.class = 1
WHERE
    SCHEMA_NAME(t.schema_id) = 'dbo'
    AND t.name = @TableName
    AND p.name = 'MS_Description'", table.Name));

            foreach (var row in reader.Rows)
            {
                var column = table.Columns.FirstOrDefault(c => c.Key == row.GetAs<string>("ColumnName")).Value;
                if (column != null)
                {
                    var description = row.GetAs<string>("Property");
                    var descriptionProperty = new SqlColumnDescription(column, description);
                    column.Properties.Add(descriptionProperty);
                }
            }
        }

        private readonly string SqlGetTableDocumentation = @"
SELECT
    t.name TableName, 
    p.value Property
FROM
    sys.tables AS t
    INNER JOIN sys.extended_properties AS p ON p.major_id = t.object_id AND p.minor_id = 0 AND p.class = 1
    WHERE p.name = 'MS_Description'
    AND SCHEMA_NAME(t.schema_id) = 'dbo'";

        public void AddTableDocumentation(SqlTable table)
        {
            var reader = _executer.ExecuteQuery(new SqlStatementWithParameters(
            SqlGetTableDocumentation + " AND t.name = @TableName", table.Name));

            foreach (var row in reader.Rows)
            {
                var description = row.GetAs<string>("Property");
                var descriptionProperty = new SqlTableDescription(table, description);

                table.Properties.Add(descriptionProperty);
            }
        }

        public void AddTableDocumentation(DatabaseDefinition dd)
        {
            var reader = _executer.ExecuteQuery(@"
SELECT
    t.name TableName, 
    p.value Property
FROM
    sys.tables AS t
    INNER JOIN sys.extended_properties AS p ON p.major_id = t.object_id AND p.minor_id = 0 AND p.class = 1
    -- WHERE SCHEMA_NAME(t.schema_id) = 'dbo'
    -- AND t.name=''
    AND p.name = 'MS_Description'");

            foreach (var row in reader.Rows)
            {
                var table = dd.GetTables().Find(t => t.Name == row.GetAs<string>("TableName"));
                if (table != null)
                {
                    var description = row.GetAs<string>("Property");
                    var descriptionProperty = new SqlTableDescription(table, description);

                    table.Properties.Add(descriptionProperty);
                }
            }
        }
    }
}
