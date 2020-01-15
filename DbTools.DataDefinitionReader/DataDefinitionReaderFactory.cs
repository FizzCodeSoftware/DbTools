namespace FizzCode.DbTools.DataDefinitionReader
{
    using System;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Common.Logger;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionExecuter;
    using FizzCode.DbTools.DataDefinitionGenerator;

    public static class DataDefinitionReaderFactory
    {
        public static IDataDefinitionReader CreateDataDefinitionReader(ConnectionStringWithProvider connectionStringWithProvider, Settings settings, Logger logger)
        {
            var sqlDialect = SqlDialectHelper.GetSqlDialectFromProviderName(connectionStringWithProvider.ProviderName);

            // TODO version detection / specify
            var version = SqlEngines.GetLatestVersion(sqlDialect);

            var context = new Context
            {
                Settings = settings,
                Logger = logger
            };

            var generator = SqlGeneratorFactory.CreateGenerator(version, context);

            var executer = SqlExecuterFactory.CreateSqlExecuter(connectionStringWithProvider, generator);

            return CreateDataDefinitionReader(version.SqlDialect, executer);
        }

        public static IDataDefinitionReader CreateDataDefinitionReader(SqlDialectX sqlDialect, SqlExecuter sqlExecuter)
        {
            return sqlDialect switch
            {
                SqlDialectX.MsSql => new MsSqlDataDefinitionReader(sqlExecuter),
                SqlDialectX.Oracle => new OracleDataDefinitionReader(sqlExecuter),
                _ => throw new NotImplementedException($"Not implemented {sqlDialect}."),
            };
        }
    }
}