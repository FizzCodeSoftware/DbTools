namespace FizzCode.DbTools.QueryBuilder
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using FizzCode.DbTools.DataDefinition;

    public abstract class JoinBase : QueryElement
    {
        protected JoinBase(SqlTable table, string alias, JoinType joinType, params QueryColumn[] columns)
            : base(table, alias, columns)
        {
            JoinType = joinType;
        }

        protected JoinBase(SqlTable table, JoinType joinType, params QueryColumn[] columns)
            : base(table, columns)
        {
            JoinType = joinType;
        }

        public JoinType JoinType { get; set; }

        public override string ToString()
        {
#pragma warning disable IDE0071 // Simplify interpolation
            return $"{JoinType.ToString()}Join {Table.SchemaAndTableName} AS {Table.GetAlias()}";
#pragma warning restore IDE0071 // Simplify interpolation
        }
    }

    public class Join : JoinBase
    {
        public Join(SqlTable table, QueryColumn columnTo, QueryColumn columnFrom, string alias, JoinType joinType, params QueryColumn[] columns)
            : base(table, alias, joinType, columns)
        {
            ColumnTo = columnTo;
            ColumnFrom = columnFrom;
        }

        public QueryColumn ColumnTo { get; set; }
        public QueryColumn ColumnFrom { get; set; }
    }

    public class JoinOn : JoinBase
    {
        public JoinOn(SqlTable table, Expression on, string alias, JoinType joinType, params QueryColumn[] columns)
            : base(table, alias, joinType, columns)
        {
            OnExpression = on;
        }

        public JoinOn(SqlTable table, Expression on, JoinType joinType, params QueryColumn[] columns)
            : base(table, joinType, columns)
        {
            OnExpression = on;
        }

        public Expression OnExpression { get; set; }
    }

    public class Expression : IEnumerable<object>
    {
        public List<object> Values { get; } = new List<object>();
        public Expression(params object[] expressionParts)
        {
            Values = expressionParts.ToList();
        }

        /*public static implicit operator Expression(object[] expressionParts)
        {
            var expression = new Expression(expressionParts);
            return expression;
        }*/

        public IEnumerator<object> GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        public static string GetExpression(IEnumerable<object> expressionParts, IEnumerable<QueryElement> queryElements, QueryElement mainQueryElement = null)
        {
            var sb = new StringBuilder();
            string previous = null;

            foreach (var obj in expressionParts)
            {
                if (obj is Expression expression)
                {
                    sb.AppendSpace(GetExpression(expression.Values, queryElements, mainQueryElement));
                }
                else if (obj is SqlColumn sqlColumn)
                {
                    if (previous?.EndsWith('.') != true)
                    {
                        var table = sqlColumn.Table;

                        var alias = "";

                        alias = table.GetAlias();

                        if (alias == null)
                        {
                            alias = mainQueryElement?.Table.SchemaAndTableName == table.SchemaAndTableName
                            ? mainQueryElement.Table.GetAlias()
                            : queryElements.Single(qe => qe.Table.SchemaAndTableName == table.SchemaAndTableName).Table.GetAlias();
                        }

                        sb.AppendSpace(alias);
                        sb.Append(".");
                    }

                    sb.Append(((QueryColumn)sqlColumn).Value);
                    previous = null;
                }
                else if (obj is string @string)
                {
                    sb.AppendSpace(@string);
                    previous = @string;
                }
                else
                {
                    throw new ArgumentException($"Expression part type is not handled. Type: {obj.GetType()}, Value: {obj}.");
                }
            }

            return sb.ToString();
        }
    }
}
