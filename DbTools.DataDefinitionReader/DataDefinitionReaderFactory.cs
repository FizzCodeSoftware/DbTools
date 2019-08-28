namespace FizzCode.DbTools.DataDefinitionReader
{
    using System;
    using System.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionExecuter;
    using FizzCode.DbTools.DataDefinitionGenerator;

    public static class DataDefinitionReaderFactory
    {
        public static IDataDefinitionReader CreateDataDefinitionReader(ConnectionStringSettings connectionStringSettings)
        {
            var sqlDialect = SqlDialectHelper.GetSqlDialectFromConnectionStringSettings(connectionStringSettings);
            var generator = SqlGeneratorFactory.CreateGenerator(sqlDialect);
            var executer = SqlExecuterFactory.CreateSqlExecuter(connectionStringSettings, generator, null); // no specific settings for reader - for now

            return CreateDataDefinitionReader(sqlDialect, executer);
        }

        public static IDataDefinitionReader CreateDataDefinitionReader(SqlDialect sqlDialect, SqlExecuter sqlExecuter)
        {
            switch (sqlDialect)
            {
                case SqlDialect.MsSql:
                    return new MsSqlDataDefinitionReader(sqlExecuter);
                case SqlDialect.SqLite:
                    return new SqLiteDataDefinitionReader(sqlExecuter);
                default:
                    throw new NotImplementedException($"Not implemented {sqlDialect}.");
            }
        }
    }
}