namespace FizzCode.DbTools.DataDefinitionReader
{
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionExecuter;

    public class MsSqlIdentityReader
    {
        public MsSqlIdentityReader(SqlExecuter sqlExecuter)
        {
            _executer = sqlExecuter;
        }

        protected readonly SqlExecuter _executer;

        private List<Row> _queryResult;

        private List<Row> QueryResult
        {
            get
            {
                if (_queryResult == null)
                {
                    var reader = _executer.ExecuteQuery(GetIdentitySql(true));

                    _queryResult = reader.Rows;
                }

                return _queryResult;
            }
        }

        public void GetIdentity(DatabaseDefinition dd)
        {
            foreach (var table in dd.GetTables())
                GetIdentity(table);
        }

        public void GetIdentity(SqlTable table)
        {
            foreach (var row in QueryResult.Where(row => DataDefinitionReaderHelper.SchemaAndTableNameEquals(row, table)))
            {
                var column = table.Columns[row.GetAs<string>("column_name")];

                if (!column.Properties.OfType<Identity>().Any())
                {
                    column.Properties.Add(new Identity(column));
                }
            }
        }

        private string GetIdentitySql(bool isPrimaryKey)
        {
            return $@"
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
