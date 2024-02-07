using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.QueryBuilder;
public class Expression(params object[] expressionParts)
    : IEnumerable<object>
{
    public List<object> Values { get; } = expressionParts.ToList();

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

    public static string GetExpression(IEnumerable<object> expressionParts, IEnumerable<QueryElement> queryElements, QueryElement? mainQueryElement = null)
    {
        var sb = new StringBuilder();
        string? previous = null;

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

                    alias ??= mainQueryElement?.Table.SchemaAndTableName == table.SchemaAndTableName
                        ? mainQueryElement.Table.GetAlias()
                        : queryElements.Single(qe => qe.Table.SchemaAndTableName == table.SchemaAndTableName).Table.GetAlias();

                    sb.AppendSpace(alias);
                    sb.Append('.');
                }

                sb.Append(((QueryColumn)sqlColumn).Value);
                previous = null;
            }
            else if (obj is string @string)
            {
                sb.AppendSpace(@string);
                previous = @string;
            }
            else if (obj is int @int)
            {
                sb.AppendSpace(@int.ToString(CultureInfo.InvariantCulture));
                // previous = @int;
            }
            else
            {
                throw new ArgumentException($"Expression part type is not handled. Type: {obj.GetType()}, Value: {obj}.");
            }
        }

        return sb.ToString();
    }
}
