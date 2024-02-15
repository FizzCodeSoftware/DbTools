using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.QueryBuilder.Interfaces;

namespace FizzCode.DbTools.QueryBuilder;
public class StoredProcedureFromQuery(Query query, params SqlParameter[] spParameters)
    : StoredProcedure(spParameters), IStoredProcedureFromQuery
{
    public Query Query { get; } = query;
}
