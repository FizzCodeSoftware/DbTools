namespace FizzCode.DbTools.DataDefinitionExecuter
{
    using System;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionGenerator;

    public static class SqlExecuterFactory
    {
        public static SqlExecuter CreateSqlExecuter(ConnectionStringWithProvider connectionStringWithProvider, ISqlGenerator sqlGenerator)
        {
            var dialect = SqlDialectHelper.GetSqlDialectFromProviderName(connectionStringWithProvider.ProviderName);

            return dialect switch
            {
                SqlDialect.SqLite => new SqLiteExecuter(connectionStringWithProvider, sqlGenerator),
                SqlDialect.MsSql => new MsSqlExecuter(connectionStringWithProvider, sqlGenerator),
                SqlDialect.Oracle => new OracleExecuter(connectionStringWithProvider, sqlGenerator),
                _ => throw new NotImplementedException($"Not implemented {dialect}."),
            };
        }
    }
}