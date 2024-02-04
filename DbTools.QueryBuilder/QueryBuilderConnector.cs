using System;
using System.Collections.Generic;
using FizzCode.DbTools.Common;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.QueryBuilder.Interfaces;
using FizzCode.DbTools.SqlGenerator.Base;

namespace FizzCode.DbTools.QueryBuilder;
public class QueryBuilderConnector : AbstractSqlGeneratorBase, IQueryBuilderConnector
{
    protected IQueryBuilder QueryBuilder { get; }
    public QueryBuilderConnector(Context context, IQueryBuilder queryBuilder)
        : base(context)
    {
        QueryBuilder = queryBuilder;
    }

    public override SqlEngineVersion SqlVersion
    {
        get
        {
            return QueryBuilder.SqlVersion;
        }
    }

    public void ProcessStoredProcedureFromQuery(IStoredProcedureFromQuery storedProcedureFromQuery)
    {
        var sp = storedProcedureFromQuery as StoredProcedureFromQuery;

        foreach (var p in GetParametersFromFilters(sp.Query))
            sp.SpParameters.Add(p);

        sp.StoredProcedureBodies.Add(SqlVersion, Build(sp.Query));
    }

    public void ProcessViewFromQuery(IViewFromQuery viewFromQuery)
    {
        var view = viewFromQuery as ViewFromQuery;
        view.SqlViewBodies.Add(SqlVersion, Build(view.Query));
    }

    public string Build(Query query)
    {
        return QueryBuilder.Build(query);
    }

    public List<SqlParameter> GetParametersFromFilters(Query query)
    {
        var parameters = new List<SqlParameter>();

        foreach (var filter in query.Filters)
        {
            switch (filter.Type)
            {
                case FilterType.Between:
                    {
                        parameters.Add(filter.Parameter.Copy("min" + filter.Column.Value));
                        parameters.Add(filter.Parameter.Copy("max" + filter.Column.Value));
                        break;
                    }

                case FilterType.Equal:
                case FilterType.Greater:
                case FilterType.Lesser:
                    parameters.Add(filter.Parameter);
                    break;
                default:
#pragma warning disable IDE0071 // Simplify interpolation
                    throw new NotImplementedException($"Filtering for {filter.Type.ToString()} is not implemented.");
#pragma warning restore IDE0071 // Simplify interpolation
            }
        }

        return parameters;
    }
    public override string GuardKeywordsImplementation(string name)
    {
        throw new NotImplementedException();
    }
}
