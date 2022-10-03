namespace FizzCode.DbTools.Factory.Interfaces
{
    using FizzCode.DbTools.QueryBuilder.Interfaces;

    public interface IQueryBuilderFactory
    {
        IQueryBuilderConnector CreateQueryBuilderFactory();
    }
}