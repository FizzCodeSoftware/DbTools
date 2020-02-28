namespace FizzCode.DbTools.DataDefinitionReader
{
    using System;
    using System.Linq;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.SqlExecuter;

    public class MsSqlColumnDocumentationReader2016 : GenericDataDefinitionElementReader
    {
        private ILookup<string, Row> _queryResult;
        private ILookup<string, Row> QueryResult => _queryResult ?? (_queryResult = Executer.ExecuteQuery(GetStatement()).Rows.ToLookup(x => x.GetAs<string>("SchemaAndTableName")));

        public MsSqlColumnDocumentationReader2016(SqlStatementExecuter executer, SchemaNamesToRead schemaNames = null)
            : base(executer, schemaNames)
        {
        }

        public void GetColumnDocumentation(SqlTable table)
        {
            var defaultSchema = Executer.Generator.Context.Settings.SqlVersionSpecificSettings.GetAs<string>("DefaultSchema");
            var schemaAndTableName = (table.SchemaAndTableName.Schema ?? defaultSchema) + "." + table.SchemaAndTableName.TableName;
            var rows = QueryResult[schemaAndTableName];

            foreach (var row in rows)
            {
                var columnName = row.GetAs<string>("ColumnName");
                if (table.Columns.TryGetValue(columnName, out var column))
                {
                    var description = row.GetAs<string>("Property");
                    if (!string.IsNullOrEmpty(description))
                    {
                        description = description.Replace("\\n", "\n", StringComparison.OrdinalIgnoreCase).Trim();
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
    CONCAT(SCHEMA_NAME(t.schema_id), '.', t.name) SchemaAndTableName,
    c.name ColumnName,
    p.value Property
FROM
    sys.tables t
    INNER JOIN sys.all_columns c ON c.object_id = t.object_id
    INNER JOIN sys.extended_properties p ON p.major_id = t.object_id AND p.minor_id = c.column_id AND p.class = 1
WHERE
    p.name = 'MS_Description'";
        }
    }
}