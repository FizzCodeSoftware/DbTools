using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.QueryBuilder.Interfaces;

namespace FizzCode.DbTools.QueryBuilder;
public class ViewFromQuery : SqlView, IViewFromQuery
{
    public ViewFromQuery(Query query)
    {
        Query = query;
    }

    public Query Query { get; set; }
}
