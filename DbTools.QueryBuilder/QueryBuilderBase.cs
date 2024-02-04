using System.Linq;
using System.Text;

namespace FizzCode.DbTools.QueryBuilder;
public abstract class QueryBuilderBase
{
    protected Query _query;

    protected bool HasColumnWithSameName(QueryElement queryElement, QueryColumn queryColumn)
    {
        var columns = queryElement.GetColumns();
        if (columns == null)
            return false;

        return columns.Any(c => c.Value == queryColumn.Value);
    }

    protected string AddJoinColumns()
    {
        var sb = new StringBuilder();

        for (var i = 0; i < _query.Joins.Count; i++)
        {
            if (i == 0 && _query.QueryColumns.Count == 1 && _query.QueryColumns[0] is None)
                sb.Append(AddQueryElementColumns(_query.Joins[i], true));
            else
                sb.AppendComma(AddQueryElementColumns(_query.Joins[i], true));
        }

        return sb.ToString();
    }

    protected string AddJoins()
    {
        var sb = new StringBuilder();

        foreach (var join in _query.Joins)
            sb.Append(AddJoin(join));

        return sb.ToString();
    }

    protected string AddJoinOn(JoinOn joinOn)
    {
        return Expression.GetExpression(joinOn.OnExpression, _query.QueryElements, joinOn);
    }

    protected abstract string AddQueryElementColumns(QueryElement queryElement, bool useAlias = false);
    protected abstract string AddJoin(JoinBase join);
}
