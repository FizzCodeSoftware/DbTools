using System;
using FizzCode.DbTools.DataDefinition.Base.Interfaces;
using FizzCode.DbTools.DataDefinition.MsSql2016;
using FizzCode.DbTools.DataDefinition.Oracle12c;
using FizzCode.DbTools.Factory.Interfaces;
using FizzCode.LightWeight.AdoNet;

namespace FizzCode.DbTools.DataDefinition.Factory;
public class DataDefinitionReaderFactory : IDataDefinitionReaderFactory
{
    private readonly IContextFactory _contextFactory;
    public DataDefinitionReaderFactory(IContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public IDataDefinitionReader CreateDataDefinitionReader(NamedConnectionString connectionString, ISchemaNamesToRead schemaNames)
    {
        var sqlEngineVersion = connectionString.GetSqlEngineVersion();
        var context = _contextFactory.CreateContextWithLogger(sqlEngineVersion);

        if (sqlEngineVersion is MsSqlVersion)
            return new MsSql2016DataDefinitionReader(connectionString, context, schemaNames);

        if (sqlEngineVersion is OracleVersion)
            return new Oracle12cDataDefinitionReader(connectionString, context, schemaNames);

        throw new NotImplementedException($"Not implemented {sqlEngineVersion}.");
    }
}