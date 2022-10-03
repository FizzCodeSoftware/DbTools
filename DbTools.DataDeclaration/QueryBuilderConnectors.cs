namespace FizzCode.DbTools.DataDeclaration
{
    using FizzCode.DbTools.Factory.Collections;
    using FizzCode.DbTools.Factory.Interfaces;
    using FizzCode.DbTools.QueryBuilder.Interfaces;

    public class QueryBuilderConnectors : SqlEngineVersionFactoryDictionary<IQueryBuilderConnector, IQueryBuilderFactory>
    {
        public QueryBuilderConnectors(IQueryBuilderFactory factory)
            : base(factory)
        {
        }

        protected override IQueryBuilderConnector Create(SqlEngineVersion version)
        {
            return _factory.CreateQueryBuilderConnector(version);
        }

        public void Add(IQueryBuilderConnector queryBuilderConnector)
        {
            Add(queryBuilderConnector.SqlVersion, queryBuilderConnector);
        }
    }
}
