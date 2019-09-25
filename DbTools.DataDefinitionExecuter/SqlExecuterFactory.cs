namespace FizzCode.DbTools.DataDefinitionExecuter
{
    using System;
    using System.Configuration;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionGenerator;

    public static class SqlExecuterFactory
    {
        public static SqlExecuter CreateSqlExecuter(ConnectionStringSettings connectionStringSettings, ISqlGenerator sqlGenerator)
        {
            var dialect = SqlDialectHelper.GetSqlDialectFromConnectionStringSettings(connectionStringSettings);

            return dialect switch
            {
                SqlDialect.SqLite => new SqLiteExecuter(connectionStringSettings, sqlGenerator),
                SqlDialect.MsSql => new MsSqlExecuter(connectionStringSettings, sqlGenerator),
                SqlDialect.Oracle => new OracleExecuter(connectionStringSettings, sqlGenerator),
                _ => throw new NotImplementedException($"Not implemented {dialect}."),
            };
        }
    }
}