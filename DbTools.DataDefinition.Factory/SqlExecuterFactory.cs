using System;
using FizzCode.LightWeight;
using FizzCode.DbTools.SqlExecuter.MsSql;
using FizzCode.DbTools.SqlExecuter.Oracle;
using FizzCode.DbTools.SqlExecuter.SqLite3;
using FizzCode.DbTools.Factory.Interfaces;
using FizzCode.DbTools.Interfaces;

namespace FizzCode.DbTools.DataDefinition.Factory;
public class SqlExecuterFactory : ISqlExecuterFactory
{
    private readonly ISqlGeneratorFactory _sqlGeneratorFactory;
    private readonly IContextFactory _contextFactory;

    public SqlExecuterFactory(IContextFactory contextFactory, ISqlGeneratorFactory sqlGeneratorFactory)
    {
        _contextFactory = contextFactory;
        _sqlGeneratorFactory = sqlGeneratorFactory;
    }

    protected ISqlStatementExecuter CreateSqlExecuter(NamedConnectionString connectionString, ISqlGenerator sqlGenerator)
    {
        var sqlEngineVersion = connectionString.GetSqlEngineVersion();
        var context = _contextFactory.CreateContextWithLogger(sqlEngineVersion);

        if (sqlEngineVersion == SqLiteVersion.SqLite3)
            return new SqLite3Executer(context, connectionString, sqlGenerator);

        if (sqlEngineVersion == OracleVersion.Oracle12c)
            return new Oracle12cExecuter(context, connectionString, sqlGenerator);

        if (sqlEngineVersion == MsSqlVersion.MsSql2016)
            return new MsSql2016Executer(context, connectionString, sqlGenerator);

        throw new NotImplementedException($"Not implemented {sqlEngineVersion}.");
    }

    public ISqlStatementExecuter CreateSqlExecuter(NamedConnectionString connectionString)
    {
        var sqlEngineVersion = connectionString.GetSqlEngineVersion();

        var generator =_sqlGeneratorFactory.CreateSqlGenerator(sqlEngineVersion);

        return CreateSqlExecuter(connectionString, generator);
    }
}