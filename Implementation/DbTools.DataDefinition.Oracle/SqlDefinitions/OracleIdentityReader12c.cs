namespace FizzCode.DbTools.DataDefinitionReader
{
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Base;
    using FizzCode.DbTools.DataDefinition.Base.Interfaces;
    using FizzCode.DbTools.SqlExecuter;

    public class OracleIdentityReader12c : OracleDataDefinitionElementReader
    {
        private readonly List<Row> _queryResult;

        public OracleIdentityReader12c(SqlStatementExecuter executer, ISchemaNamesToRead schemaNames)
            : base(executer, schemaNames)
        {
            var sqlStatement = GetStatement();
            AddSchemaNamesFilter(ref sqlStatement, "owner");
            _queryResult = Executer.ExecuteQuery(sqlStatement).Rows.ToList();
        }

        public void GetIdentity(DatabaseDefinition dd)
        {
            foreach (var table in dd.GetTables())
                GetIdentity(table);
        }

        public void GetIdentity(SqlTable table)
        {
            var rows = _queryResult
                .Where(row => DataDefinitionReaderHelper.SchemaAndTableNameEquals(row, table, "OWNER", "TABLE_NAME"))
                .ToList();

            foreach (var row in rows)
            {
                var column = table.Columns[row.GetAs<string>("COLUMN_NAME")];
                column.Properties.Add(new Identity(column));
            }
        }

        private static string GetStatement()
        {
            return @"
SELECT owner,
    table_name, 
    column_name,
    generation_type,
    identity_options
FROM all_tab_identity_cols
WHERE 1=1";
        }
    }
}
