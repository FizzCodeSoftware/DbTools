namespace FizzCode.DbTools.QueryBuilder
{
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.QueryBuilder.Interface;

    public class ViewFromQuery : SqlView, IViewFromQuery
    {
        public ViewFromQuery(Query query)
        {
            Query = query;
        }

        public Query Query { get; set; }
    }
}
