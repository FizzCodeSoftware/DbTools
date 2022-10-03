namespace FizzCode.DbTools.QueryBuilder
{
    using FizzCode.DbTools.Factory.Interfaces;
    using FizzCode.DbTools.QueryBuilder.Interfaces;

    public class QueryBuilderFactory : IQueryBuilderFactory
    {
        protected IContextFactory ContextFactory { get; }
        protected ISqlGeneratorBaseFactory SqlGeneratorBaseFactory { get; }

        public QueryBuilderFactory(IContextFactory contextFactory, ISqlGeneratorBaseFactory sqlGeneratorBaseFactory)
        {
            ContextFactory = contextFactory;
            SqlGeneratorBaseFactory = sqlGeneratorBaseFactory;
        }

        public IQueryBuilderConnector CreateQueryBuilderConnector(SqlEngineVersion version)
        {
            var context = ContextFactory.CreateContext(version);
            return new QueryBuilderConnector(context, CreateQueryBuilder(version));
        }

        public IQueryBuilder CreateQueryBuilder(SqlEngineVersion version)
        {
            var sqlGeneratorBase = SqlGeneratorBaseFactory.CreateGenerator(version);
            return new QueryBuilderSqlGeneratorBase(sqlGeneratorBase);
        }
    }
}
