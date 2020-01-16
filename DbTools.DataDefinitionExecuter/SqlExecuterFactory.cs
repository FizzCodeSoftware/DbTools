namespace FizzCode.DbTools.DataDefinitionExecuter
{
    using System;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinitionGenerator;

    public static class SqlExecuterFactory
    {
        public static SqlExecuter CreateSqlExecuter(ConnectionStringWithProvider connectionStringWithProvider, ISqlGenerator sqlGenerator)
        {
            if (connectionStringWithProvider.SqlEngineVersion is SqLite3)
                return new SqLiteExecuter3(connectionStringWithProvider, sqlGenerator);

            if (connectionStringWithProvider.SqlEngineVersion is Oracle12c)
                return new OracleExecuter12c(connectionStringWithProvider, sqlGenerator);

            if (connectionStringWithProvider.SqlEngineVersion is MsSql2016)
                return new MsSqlExecuter2016(connectionStringWithProvider, sqlGenerator);

            throw new NotImplementedException($"Not implemented {connectionStringWithProvider.SqlEngineVersion}.");
        }
    }
}