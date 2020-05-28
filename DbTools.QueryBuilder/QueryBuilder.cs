namespace FizzCode.DbTools.QueryBuilder
{
    using System.Collections.Generic;
    using FizzCode.DbTools.DataDefinition;

    public class QueryBuilder
    {
        public string Build(Query query)
        {
            return "";
        }
    }

    public class Query : QueryElement
    {
    }

    public class Join
    {
    }

    public class QueryElement
    {

        public SqlTable Table { get; set; }
        public List<SqlColumn> QueryColumns { get; set; }
    }
}
