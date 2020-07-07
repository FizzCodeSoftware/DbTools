namespace FizzCode.DbTools.QueryBuilder.Interface
{
    using System.Collections.Generic;

    public interface IQuery
    {
    }

    public interface IQueryBuilder
    {
        string Build(IQuery query);
        List<ISqlParameter> GetParamtersFromFilters(IQuery query);
    }

    public interface ISqlParameter
    {
    }
}


