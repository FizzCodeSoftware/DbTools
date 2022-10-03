namespace FizzCode.DbTools.QueryBuilder
{
    using FizzCode.DbTools.DataDefinition.Base;
    using FizzCode.DbTools.QueryBuilder.Interfaces;

    public class ViewFromQuery : SqlView, IViewFromQuery
    {
        public ViewFromQuery(Query query)
        {
            Query = query;
        }

        public Query Query { get; set; }
    }
}
