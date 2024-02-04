using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.QueryBuilder.Interfaces;

namespace FizzCode.DbTools.QueryBuilder;
public class StoredProcedureFromQuery : StoredProcedure, IStoredProcedureFromQuery
{
    public StoredProcedureFromQuery(Query query, params SqlParameter[] spParameters)
        : base(null, spParameters)
    {
        Query = query;
    }

    public Query Query { get; set; }
}
