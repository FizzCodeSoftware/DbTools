namespace FizzCode.DbTools.QueryBuilder
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using FizzCode.DbTools.DataDefinition;

    public class QueryBuilder
    {
        public string Build(Query query)
        {
            var sb = new StringBuilder();

            sb.Append("SELECT ");
            sb.Append(string.Join(", ", query.Table.Columns.Select(c => c.Name)));
            sb.Append("FROM ");
            sb.Append(QueryHelper.GetSimplifiedSchemaAndTableName(query.Table.SchemaAndTableName));

            return sb.ToString();
        }
    }

    public class Query : QueryElement
    {
        public List<Join> Joins { get; } = new List<Join>();

        public Query Join(Join join)
        {
            Joins.Add(join);
            return this;
        }
    }

    public class Join : QueryElement
    {
        // public SqlColumn 
    }

    public class QueryElement
    {
        public SqlTable Table { get; set; }
        public List<SqlColumn> QueryColumns { get; set; }
    }

    public static class QueryHelper
    {
        // TODO temporal for now, duplicated
        public static string GetSimplifiedSchemaAndTableName(SchemaAndTableName schemaAndTableName, string separator = ".")
        {
            var schema = schemaAndTableName.Schema;
            var tableName = schemaAndTableName.TableName;

            //var defaultSchema = Settings.SqlVersionSpecificSettings.GetAs<string>("DefaultSchema", null);
            var defaultSchema = "dbo";

            /*if (Settings.Options.ShouldUseDefaultSchema && schema == null)
                return defaultSchema + separator + tableName;

            if (!Settings.Options.ShouldUseDefaultSchema && schema == defaultSchema)
                return tableName;*/

            if (schema == defaultSchema)
                return tableName;

            if (schema != null)
                return schema + separator + tableName;

            return tableName;
        }
    }
}
