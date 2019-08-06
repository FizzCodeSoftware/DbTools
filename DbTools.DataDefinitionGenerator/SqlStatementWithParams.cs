namespace FizzCode.DbTools.DataDefinitionGenerator
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public class SqlStatementWithParameters
    {
        public SqlStatementWithParameters(string statement, params object[] paramValues)
        {
            Statement = statement;

            var matches = Regex.Matches(statement, @"\B\@\w+");
            var i = 0;

            foreach (var paramValue in paramValues)
            {
                Parameters.Add(matches[i++].Value, paramValue);
            }
        }

        public string Statement { get; set; }
        public Dictionary<string, object> Parameters { get; } = new Dictionary<string, object>();

        public static implicit operator SqlStatementWithParameters(string statement)
        {
            return new SqlStatementWithParameters(statement);
        }
    }
}