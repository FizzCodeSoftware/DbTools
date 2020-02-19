namespace FizzCode.DbTools.DataDefinition
{
    using System;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition.MsSql2016;
    using FizzCode.DbTools.DataDefinition.Oracle12c;
    using FizzCode.DbTools.DataDefinition.SqlExecuter;
    using FizzCode.DbTools.DataDefinition.SqlGenerator;
    using FizzCode.DbTools.DataDefinition.SqLite3;

    public static class SqlExecuterFactory
    {
        public static SqlStatementExecuter CreateSqlExecuter(ConnectionStringWithProvider connectionStringWithProvider, ISqlGenerator sqlGenerator)
        {
            if (connectionStringWithProvider.SqlEngineVersion == SqLiteVersion.SqLite3)
                return new SqLite3Executer(connectionStringWithProvider, sqlGenerator);

            if (connectionStringWithProvider.SqlEngineVersion == OracleVersion.Oracle12c)
                return new Oracle12cExecuter(connectionStringWithProvider, sqlGenerator);

            if (connectionStringWithProvider.SqlEngineVersion == MsSqlVersion.MsSql2016)
                return new MsSql2016Executer(connectionStringWithProvider, sqlGenerator);

            throw new NotImplementedException($"Not implemented {connectionStringWithProvider.SqlEngineVersion}.");
        }

        public static SqlStatementExecuter CreateSqlExecuter(ConnectionStringWithProvider connectionStringWithProvider, Context context)
        {
            var generator = SqlGeneratorFactory.CreateGenerator(connectionStringWithProvider.SqlEngineVersion, context);

            return CreateSqlExecuter(connectionStringWithProvider, generator);
        }
    }
}