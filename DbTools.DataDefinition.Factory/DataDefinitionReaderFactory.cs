namespace FizzCode.DbTools.DataDefinition
{
    using System;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition.MsSql2016;
    using FizzCode.DbTools.DataDefinition.Oracle12c;
    using FizzCode.DbTools.DataDefinitionReader;

    public static class DataDefinitionReaderFactory
    {
        public static IDataDefinitionReader CreateDataDefinitionReader(ConnectionStringWithProvider connectionStringWithProvider, Context context, SchemaNamesToRead schemaNames)
        {
            if (connectionStringWithProvider.SqlEngineVersion is MsSqlVersion)
                return new MsSql2016DataDefinitionReader(connectionStringWithProvider, context, schemaNames);

            if (connectionStringWithProvider.SqlEngineVersion is OracleVersion)
                return new Oracle12cDataDefinitionReader(connectionStringWithProvider, context, schemaNames);

            throw new NotImplementedException($"Not implemented {connectionStringWithProvider.SqlEngineVersion}.");
        }
    }
}