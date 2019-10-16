namespace FizzCode.DbTools.DataDefinitionReader
{
    using System;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionExecuter;
    using FizzCode.DbTools.DataDefinitionGenerator;

    public static class DataDefinitionReaderFactory
    {
        public static IDataDefinitionReader CreateDataDefinitionReader(ConnectionStringWithProvider connectionStringWithProvider, Settings settings)
        {
            var sqlDialect = SqlDialectHelper.GetSqlDialectFromProviderName(connectionStringWithProvider.ProviderName);
            var generator = SqlGeneratorFactory.CreateGenerator(sqlDialect, settings);
            var executer = SqlExecuterFactory.CreateSqlExecuter(connectionStringWithProvider, generator);

            return CreateDataDefinitionReader(sqlDialect, executer);
        }

        public static IDataDefinitionReader CreateDataDefinitionReader(SqlDialect sqlDialect, SqlExecuter sqlExecuter)
        {
            return sqlDialect switch
            {
                SqlDialect.MsSql => new MsSqlDataDefinitionReader(sqlExecuter),
                SqlDialect.SqLite => new SqLiteDataDefinitionReader(sqlExecuter),
                _ => throw new NotImplementedException($"Not implemented {sqlDialect}."),
            };
        }
    }
}