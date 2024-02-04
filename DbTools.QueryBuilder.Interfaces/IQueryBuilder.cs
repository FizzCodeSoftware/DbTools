namespace FizzCode.DbTools.QueryBuilder.Interfaces;

public interface IQueryBuilder
{
    string Build(IQuery query);
    SqlEngineVersion SqlVersion { get; }
}