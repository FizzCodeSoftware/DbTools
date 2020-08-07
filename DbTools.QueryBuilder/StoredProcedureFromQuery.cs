namespace FizzCode.DbTools.QueryBuilder
{
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.QueryBuilder.Interface;

    public class StoredProcedureFromQuery : StoredProcedure, IStoredProcedureFromQuery
    {
        public StoredProcedureFromQuery(Query query, params SqlParameter[] spParameters)
            : base(null, spParameters)
        {
            Query = query;
        }

        public Query Query { get; set; }
    }
}
