namespace FizzCode.DbTools.DataDefinitionReader
{
    using System;
    using System.Configuration;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionExecuter;
    using FizzCode.DbTools.DataDefinitionGenerator;

    public static class DataDefinitionReaderFactory
    {
        public static IDataDefinitionReader CreateDataDefinitionReader(ConnectionStringSettings connectionStringSettings, Settings settings)
        {
            var sqlDialect = SqlDialectHelper.GetSqlDialectFromConnectionStringSettings(connectionStringSettings);
            var generator = SqlGeneratorFactory.CreateGenerator(sqlDialect, settings);
            var executer = SqlExecuterFactory.CreateSqlExecuter(connectionStringSettings, generator);

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