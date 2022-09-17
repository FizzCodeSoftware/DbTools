namespace FizzCode.DbTools.DataDefinitionReader
{
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Base;
    using FizzCode.DbTools.SqlExecuter;

    public class MsSqlIdentityReader2016 : GenericDataDefinitionElementReader
    {
        private List<Row> _queryResult;
        private List<Row> QueryResult => _queryResult ??= Executer.ExecuteQuery(GetStatement()).Rows;

        public MsSqlIdentityReader2016(SqlStatementExecuter executer, SchemaNamesToRead schemaNames)
            : base(executer, schemaNames)
        {
        }

        public void GetIdentity(DatabaseDefinition dd)
        {
            foreach (var table in dd.GetTables())
                GetIdentity(table);
        }

        public void GetIdentity(SqlTable table)
        {
            var rows = QueryResult
                .Where(row => DataDefinitionReaderHelper.SchemaAndTableNameEquals(row, table));

            foreach (var row in rows)
            {
                var column = table.Columns[row.GetAs<string>("column_name")];
                column.Properties.Add(new Identity(column));
            }
        }

        private static string GetStatement()
        {
            return @"
SELECT schema_name(tab.schema_id) schema_name, 
    col.[name] as column_name, 
    tab.[name] as table_name
FROM sys.tables tab
    INNER JOIN sys.columns col
        ON tab.object_id = col.object_id
WHERE is_identity = 1";
        }
    }
}
