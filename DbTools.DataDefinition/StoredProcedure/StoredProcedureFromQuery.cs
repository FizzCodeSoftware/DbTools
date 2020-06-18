namespace FizzCode.DbTools.DataDefinition
{
    using FizzCode.DbTools.QueryBuilder.Interface;

    public class StoredProcedureFromQuery : StoredProcedure
    {
        public StoredProcedureFromQuery(IQuery query, params SpParameter[] spParameters)
            : base(null, spParameters)
        {
            Query = query;
        }

        public IQuery Query { get; set; }
    }
}
