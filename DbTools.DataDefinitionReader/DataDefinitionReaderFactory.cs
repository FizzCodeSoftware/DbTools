namespace FizzCode.DbTools.DataDefinitionReader
{
    using System;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Common.Logger;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinitionExecuter;
    using FizzCode.DbTools.DataDefinitionGenerator;

    public static class DataDefinitionReaderFactory
    {
        public static IDataDefinitionReader CreateDataDefinitionReader(ConnectionStringWithProvider connectionStringWithProvider, Settings settings, Logger logger)
        {
            var context = new Context
            {
                Settings = settings,
                Logger = logger
            };

            var generator = SqlGeneratorFactory.CreateGenerator(connectionStringWithProvider.SqlEngineVersion, context);

            var executer = SqlExecuterFactory.CreateSqlExecuter(connectionStringWithProvider, generator);

            return CreateDataDefinitionReader(connectionStringWithProvider.SqlEngineVersion, executer);
        }

        public static IDataDefinitionReader CreateDataDefinitionReader(SqlVersion version, SqlExecuter sqlExecuter)
        {
            if (version is IMsSqlDialect)
                return new MsSqlDataDefinitionReader2016(sqlExecuter);

            if (version is IOracleDialect)
                return new OracleDataDefinitionReader12c(sqlExecuter);

            throw new NotImplementedException($"Not implemented {version}.");
        }
    }
}