using System;
using FizzCode.DbTools.Common;
using FizzCode.DbTools.DataDefinition.Base.Interfaces;
using FizzCode.DbTools.DataDefinition.MsSql2016;
using FizzCode.DbTools.DataDefinition.Oracle12c;
using FizzCode.DbTools.Factory.Interfaces;
using FizzCode.LightWeight;

namespace FizzCode.DbTools.DataDefinition.Factory;
public class DataDefinitionReaderFactory(IContextFactory contextFactory) : IDataDefinitionReaderFactory
{
    private readonly IContextFactory _contextFactory = contextFactory;

    public IDataDefinitionReader CreateDataDefinitionReader(NamedConnectionString connectionString, ISchemaNamesToRead? schemaNames)
    {
        var sqlEngineVersion = Throw.IfNull(connectionString.GetSqlEngineVersion());
        var context = _contextFactory.CreateContextWithLogger(sqlEngineVersion);

        if (sqlEngineVersion is MsSqlVersion)
            return new MsSql2016DataDefinitionReader(connectionString, context, schemaNames);

        if (sqlEngineVersion is OracleVersion)
            return new Oracle12cDataDefinitionReader(connectionString, context, schemaNames);

        throw new NotImplementedException($"Not implemented {sqlEngineVersion}.");
    }
}