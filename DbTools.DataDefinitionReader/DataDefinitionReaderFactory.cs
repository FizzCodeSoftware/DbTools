namespace FizzCode.DbTools.DataDefinitionReader
{
    using System;
    using System.Collections.Generic;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;

    public static class DataDefinitionReaderFactory
    {
        public static IDataDefinitionReader CreateDataDefinitionReader(ConnectionStringWithProvider connectionStringWithProvider, Context context, List<string> schemaNames)
        {
            if (connectionStringWithProvider.SqlEngineVersion is IMsSqlDialect)
                return new MsSqlDataDefinitionReader2016(connectionStringWithProvider, context, schemaNames);

            if (connectionStringWithProvider.SqlEngineVersion is IOracleDialect)
                return new OracleDataDefinitionReader12c(connectionStringWithProvider, context, schemaNames);

            throw new NotImplementedException($"Not implemented {connectionStringWithProvider.SqlEngineVersion}.");
        }
    }
}