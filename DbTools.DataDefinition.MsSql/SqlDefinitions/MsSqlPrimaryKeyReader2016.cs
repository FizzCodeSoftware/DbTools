namespace FizzCode.DbTools.DataDefinitionReader
{
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.SqlExecuter;

    public class MsSqlPrimaryKeyReader2016 : GenericDataDefinitionElementReader
    {
        private List<Row> _queryResult;

        private List<Row> QueryResult => _queryResult ?? (_queryResult = Executer.ExecuteQuery(GetKeySql()).Rows
                        .OrderBy(row => row.GetAs<string>("schema_name"))
                        .ThenBy(row => row.GetAs<string>("index_name"))
                        .ThenBy(row => row.GetAs<int>("index_column_id"))
                        .ToList());

        public MsSqlPrimaryKeyReader2016(SqlStatementExecuter executer, List<string> schemaNames = null)
            : base(executer, schemaNames)
        {
        }

        public void GetPrimaryKey(DatabaseDefinition dd)
        {
            foreach (var table in dd.GetTables())
                GetPrimaryKey(table);
        }

        public void GetPrimaryKey(SqlTable table)
        {
            PrimaryKey pk = null;

            var rows = QueryResult
                .Where(row => DataDefinitionReaderHelper.SchemaAndTableNameEquals(row, table));

            foreach (var row in rows)
            {
                if (row.GetAs<int>("index_column_id") == 1)
                {
                    pk = new PrimaryKey(table, row.GetAs<string>("index_name"))
                    {
                        Clustered = row.GetAs<byte>("type") == 1,
                    };

                    table.Properties.Add(pk);
                }

                var column = table.Columns[row.GetAs<string>("column_name")];

                var ascDesc = row.GetAs<bool>("is_descending_key")
                    ? AscDesc.Desc
                    : AscDesc.Asc;

                pk.SqlColumns.Add(new ColumnAndOrder(column, ascDesc));
            }
        }

        private static string GetKeySql()
        {
            return @"
SELECT schema_name(tab.schema_id) schema_name, 
    i.[name] index_name,
    ic.index_column_id,
    col.[name] as column_name, 
    tab.[name] as table_name
	, i.type-- 1 CLUSTERED, 2 NONCLUSTERED
	, is_unique, is_primary_key
	, is_included_column, is_descending_key
FROM sys.tables tab
    INNER JOIN sys.indexes i
        ON tab.object_id = i.object_id 
    INNER JOIN sys.index_columns ic
        ON ic.object_id = i.object_id
        and ic.index_id = i.index_id
    INNER JOIN sys.columns col
        ON i.object_id = col.object_id
        and col.column_id = ic.column_id
WHERE is_primary_key = 1";
        }
    }
}
