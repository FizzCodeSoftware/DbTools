using FizzCode.DbTools.QueryBuilder.Interfaces;

namespace FizzCode.DbTools.Factory.Interfaces;
public interface IQueryBuilderFactory
{
    IQueryBuilderConnector CreateQueryBuilderConnector(SqlEngineVersion version);
    IQueryBuilder CreateQueryBuilder(SqlEngineVersion version);
}