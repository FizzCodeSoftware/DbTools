namespace FizzCode.DbTools.DataDefinition
{
    using System;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition.MsSql2016;
    using FizzCode.DbTools.DataDefinition.Oracle12c;
    using FizzCode.DbTools.DataDefinition.SqlExecuter;
    using FizzCode.DbTools.DataDefinition.SqlGenerator;
    using FizzCode.DbTools.DataDefinition.SqLite3;
    using FizzCode.LightWeight.AdoNet;

    public static class SqlExecuterFactory
    {
        public static SqlStatementExecuter CreateSqlExecuter(NamedConnectionString connectionStringWithProvider, ISqlGenerator sqlGenerator)
        {
            var sqlEngineVersion = connectionStringWithProvider.GetSqlEngineVersion();

            if (sqlEngineVersion == SqLiteVersion.SqLite3)
                return new SqLite3Executer(connectionStringWithProvider, sqlGenerator);

            if (sqlEngineVersion == OracleVersion.Oracle12c)
                return new Oracle12cExecuter(connectionStringWithProvider, sqlGenerator);

            if (sqlEngineVersion == MsSqlVersion.MsSql2016)
                return new MsSql2016Executer(connectionStringWithProvider, sqlGenerator);

            throw new NotImplementedException($"Not implemented {sqlEngineVersion}.");
        }

        public static SqlStatementExecuter CreateSqlExecuter(NamedConnectionString connectionStringWithProvider, Context context)
        {
            var sqlEngineVersion = connectionStringWithProvider.GetSqlEngineVersion();

            var generator = SqlGeneratorFactory.CreateGenerator(sqlEngineVersion, context);

            return CreateSqlExecuter(connectionStringWithProvider, generator);
        }
    }
}