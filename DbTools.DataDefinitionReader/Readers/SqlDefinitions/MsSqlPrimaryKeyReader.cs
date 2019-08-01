namespace FizzCode.DbTools.DataDefinitionReader
{
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionExecuter;

    public class MsSqlPrimaryKeyReader
    {
        public MsSqlPrimaryKeyReader(SqlExecuter sqlExecuter)
        {
            _executer = sqlExecuter;
        }

        protected readonly SqlExecuter _executer;

        private List<Row> _queryResult;
        private List<Row> QueryResult {
            get
            {
                if (_queryResult == null)
                {
                    var reader = _executer.ExecuteQuery(GetKeySql(true));

                    _queryResult = reader.Rows;
                }

                return _queryResult;
            }
        }

        public void GetPrimaryKey(DatabaseDefinition dd)
        {
            foreach (var table in dd.GetTables())
                GetPrimaryKey(table);
        }   

        public void GetPrimaryKey(SqlTable table)
        {
            var pk = new PrimaryKey(table, null);
            foreach (var row in QueryResult.Where(row => row.GetAs<string>("table_name") == table.Name).OrderBy(row => row.GetAs<string>("schema_name")).ThenBy(row => row.GetAs<string>("index_name")).ThenBy(row => row.GetAs<int>("index_column_id")))
            {
                if (row.GetAs<int>("index_column_id") == 1)
                {
                    pk = new PrimaryKey(table, row.GetAs<string>("index_name"));

                    if (row.GetAs<byte>("type") == 1)
                        pk.Clustered = true;

                    table.Properties.Add(pk);
                }

                var column = table.Columns[row.GetAs<string>("column_name")];

                if (row.GetAs<bool>("is_identity"))
                    column.Properties.Add(new Identity(column));

                var ascDesc = AscDesc.Asc;
                if (row.GetAs<bool>("is_descending_key"))
                    ascDesc = AscDesc.Desc;

                var columnAndOrder = new ColumnAndOrder(column, ascDesc);

                pk.SqlColumns.Add(columnAndOrder);
            }
        }

        private string GetKeySql(bool isPrimaryKey)
        {
            return $@"
SELECT schema_name(tab.schema_id) schema_name, 
    i.[name] index_name,
    ic.index_column_id,
    col.[name] as column_name, 
    tab.[name] as table_name
	, i.type-- 1 CLUSTERED, 2 NONCLUSTERED
	, is_unique, is_primary_key, is_identity
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
WHERE is_primary_key = {(isPrimaryKey ? 1 : 0)}
    --AND tab.[name] = ''
--ORDER BY schema_name(tab.schema_id),
--    i.[name],
--    ic.index_column_id";
        }
    }
}
