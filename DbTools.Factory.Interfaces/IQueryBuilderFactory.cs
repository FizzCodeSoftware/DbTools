namespace FizzCode.DbTools.Factory.Interfaces
{
    using FizzCode.DbTools.QueryBuilder.Interfaces;

    public interface IQueryBuilderFactory
    {
        IQueryBuilderConnector CreateQueryBuilderConnector(SqlEngineVersion version);
        IQueryBuilder CreateQueryBuilder(SqlEngineVersion version);
    }
}