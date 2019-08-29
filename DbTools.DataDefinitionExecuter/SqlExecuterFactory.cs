namespace FizzCode.DbTools.DataDefinitionExecuter
{
    using System;
    using System.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionGenerator;

    public static class SqlExecuterFactory
    {
        public static SqlExecuter CreateSqlExecuter(ConnectionStringSettings connectionStringSettings, ISqlGenerator sqlGenerator, SqlDialectSpecificSettings settings)
        {
            var dialect = SqlDialectHelper.GetSqlDialectFromConnectionStringSettings(connectionStringSettings);

            switch (dialect)
            {
                case SqlDialect.SqLite:
                    return new SqLiteExecuter(connectionStringSettings, sqlGenerator);
                case SqlDialect.MsSql:
                    return new MsSqlExecuter(connectionStringSettings, sqlGenerator);
                case SqlDialect.Oracle:
                    return new OracleExecuter(connectionStringSettings, sqlGenerator, settings);
                default:
                    throw new NotImplementedException($"Not implemented {dialect}.");
            }
        }
    }

    public class SqlDialectSpecificSettings : Row
    {
    }
}