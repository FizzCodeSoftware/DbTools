namespace FizzCode.DbTools.DataDefinition
{
    using System;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition.MsSql2016;
    using FizzCode.DbTools.DataDefinition.Oracle12c;
    using FizzCode.DbTools.DataDefinitionReader;
    using FizzCode.LightWeight.AdoNet;

    public static class DataDefinitionReaderFactory
    {
        public static IDataDefinitionReader CreateDataDefinitionReader(NamedConnectionString connectionStringWithProvider, Context context, SchemaNamesToRead schemaNames)
        {
            var sqlEngineVersion = connectionStringWithProvider.GetSqlEngineVersion();

            if (sqlEngineVersion is MsSqlVersion)
                return new MsSql2016DataDefinitionReader(connectionStringWithProvider, context, schemaNames);

            if (sqlEngineVersion is OracleVersion)
                return new Oracle12cDataDefinitionReader(connectionStringWithProvider, context, schemaNames);

            throw new NotImplementedException($"Not implemented {sqlEngineVersion}.");
        }
    }
}