using System.Linq;
using FizzCode.DbTools.Common;
using FizzCode.DbTools.DataDefinition;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition.Base.Interfaces;
using FizzCode.DbTools.SqlExecuter;

namespace FizzCode.DbTools.DataDefinitionReader;
public class MsSqlIdentityReader2016(SqlStatementExecuter executer, ISchemaNamesToRead schemaNames)
    : GenericDataDefinitionElementReader(executer, schemaNames)
{
    private RowSet _queryResult = null!;
    
    private RowSet QueryResult => _queryResult ??= Executer.ExecuteQuery(GetStatement());

    public void GetIdentity(DatabaseDefinition dd)
    {
        foreach (var table in dd.GetTables())
            GetIdentity(table);
    }

    public void GetIdentity(SqlTable table)
    {
        var rows = QueryResult
            .Where(row => DataDefinitionReaderHelper.SchemaAndTableNameEquals(row, table))
            .ToList();

        foreach (var row in rows)
        {
            var column = table.Columns[Throw.IfNull(row.GetAs<string>("column_name"))];
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
