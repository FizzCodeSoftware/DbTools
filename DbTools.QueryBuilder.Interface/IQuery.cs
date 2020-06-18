namespace FizzCode.DbTools.QueryBuilder.Interface
{
    public interface IQuery
    {
    }

    public interface IQueryBuilder
    {
        string Build(IQuery query);
    }
}
