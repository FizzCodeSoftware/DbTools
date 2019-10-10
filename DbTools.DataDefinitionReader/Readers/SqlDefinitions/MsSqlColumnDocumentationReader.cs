namespace FizzCode.DbTools.DataDefinitionReader
{
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionExecuter;

    public class MsSqlColumnDocumentationReader
    {
        private readonly SqlExecuter _executer;
        private List<Row> _queryResult;
        private List<Row> QueryResult => _queryResult ?? (_queryResult = _executer.ExecuteQuery(GetStatement()).Rows);

        public MsSqlColumnDocumentationReader(SqlExecuter sqlExecuter)
        {
            _executer = sqlExecuter;
        }

        public void GetColumnDocumentation(string defaultSchema, SqlTable table)
        {
            var rows = QueryResult.Where(x =>
                (x.GetAs<string>("SchemaName") == (table.SchemaAndTableName.Schema ?? defaultSchema)) &&
                (x.GetAs<string>("TableName") == table.SchemaAndTableName.TableName));

            foreach (var row in rows)
            {
                var column = table.Columns.FirstOrDefault(c => c.Key == row.GetAs<string>("ColumnName")).Value;
                if (column != null)
                {
                    var description = row.GetAs<string>("Property");
                    if (!string.IsNullOrEmpty(description))
                    {
                        description = description.Replace("\\n", "\n").Trim();
                        var descriptionProperty = new SqlColumnDescription(column, description);
                        column.Properties.Add(descriptionProperty);
                    }
                }
            }
        }

        private static string GetStatement()
        {
            return @"
SELECT
    SCHEMA_NAME(t.schema_id) SchemaName,
    t.name TableName,
    c.name ColumnName,
    p.value Property
FROM
    sys.tables t
    INNER JOIN sys.all_columns c ON c.object_id = t.object_id
    INNER JOIN sys.extended_properties p ON p.major_id = t.object_id AND p.minor_id = c.column_id AND p.class = 1
WHERE
    p.name = 'MS_Description'
    --AND SCHEMA_NAME(t.schema_id) = @SchemaName
    --AND t.name = @TableName";
        }
    }
}