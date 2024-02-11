using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace FizzCode.DbTools.Common;
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class SqlStatementWithParameters
{
    public SqlStatementWithParameters(string statement, params object?[] paramValues)
    {
        Statement = statement;

        var matches = Regex.Matches(statement, @"\B\@\w+");
        var i = 0;

        foreach (var paramValue in paramValues)
        {
            Parameters.Add(matches[i++].Value, paramValue);
        }
    }

    public SqlStatementWithParameters(string statement, Dictionary<string, object?> parameters)
    {
        Statement = statement;
        Parameters = parameters;
    }

    public string Statement { get; set; }
    public Dictionary<string, object?> Parameters { get; } = [];

    public static implicit operator SqlStatementWithParameters(string statement)
    {
        return new SqlStatementWithParameters(statement);
    }

    public string DebuggerDisplay => Statement;
}