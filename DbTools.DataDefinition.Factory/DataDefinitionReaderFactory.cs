namespace FizzCode.DbTools.DataDefinition.Factory
{
    using System;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition.MsSql2016;
    using FizzCode.DbTools.DataDefinition.Oracle12c;
    using FizzCode.DbTools.DataDefinitionReader;
    using FizzCode.LightWeight.AdoNet;

    public static class DataDefinitionReaderFactory
    {
        public static IDataDefinitionReader CreateDataDefinitionReader(NamedConnectionString connectionString, Context context, SchemaNamesToRead schemaNames)
        {
            var sqlEngineVersion = connectionString.GetSqlEngineVersion();

            if (sqlEngineVersion is MsSqlVersion)
                return new MsSql2016DataDefinitionReader(connectionString, context, schemaNames);

            if (sqlEngineVersion is OracleVersion)
                return new Oracle12cDataDefinitionReader(connectionString, context, schemaNames);

            throw new NotImplementedException($"Not implemented {sqlEngineVersion}.");
        }
    }
}