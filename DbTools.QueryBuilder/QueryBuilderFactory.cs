namespace FizzCode.DbTools.QueryBuilder
{
    using FizzCode.DbTools.Factory.Interfaces;
    using FizzCode.DbTools.QueryBuilder.Interfaces;

    public class QueryBuilderFactory : IQueryBuilderFactory
    {
        protected IContextFactory ContextFactory { get; }

        public QueryBuilderFactory(IContextFactory contextFactory)
        {
            ContextFactory = contextFactory;
        }

        public IQueryBuilderConnector CreateQueryBuilderFactory()
        {
            var context = ContextFactory.CreateContext(null);
            return new QueryBuilderConnector(context);
        }
    }
}
