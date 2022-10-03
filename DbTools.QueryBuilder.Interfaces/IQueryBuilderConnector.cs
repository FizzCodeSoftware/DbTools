namespace FizzCode.DbTools.QueryBuilder.Interfaces
{
    public interface IQueryBuilderConnector
    {
        void ProcessStoredProcedureFromQuery(IStoredProcedureFromQuery storedProcedureFromQuery);
        void ProcessViewFromQuery(IViewFromQuery viewFromQuery);
    }
}