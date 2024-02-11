using System;
using System.Data.Common;
using FizzCode.DbTools.Common;

namespace FizzCode.DbTools.SqlExecuter.Oracle;
public static class OracleDbCommandPreparer
{
    public static DbCommand PrepareSqlCommand(DbCommand dbCommand)
    {
        // Binding query parameters by name with ODP.NET
        var piBindByName = dbCommand.GetType().GetProperty("BindByName");
        Throw.InvalidOperationExceptionIfNull(piBindByName, "Oracle OracleCommand (DbCommand) BindByName property.");

        piBindByName.SetValue(dbCommand, true, null);

        ReplaceNamedParameterPrefixes(dbCommand);
        return dbCommand;
    }

    private static void ReplaceNamedParameterPrefixes(DbCommand dbCommand)
    {
#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
        dbCommand.CommandText = dbCommand.CommandText.Replace(" @", " :", StringComparison.InvariantCultureIgnoreCase)
            .Replace("(@", "(:", StringComparison.InvariantCultureIgnoreCase); //replace named parameter indicators
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities

        foreach (var paramter in dbCommand.Parameters)
        {
            var dbParameter = (DbParameter)paramter;
            dbParameter.ParameterName = ":" + dbParameter.ParameterName.TrimStart('@');
        }
    }
}