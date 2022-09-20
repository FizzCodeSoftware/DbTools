namespace FizzCode.DbTools.DataDefinition
{
    using System;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.SqlExecuter;
    using FizzCode.DbTools.DataDefinition.SqlGenerator;
    using FizzCode.LightWeight.AdoNet;
    using FizzCode.DbTools.SqlExecuter.MsSql;
    using FizzCode.DbTools.SqlExecuter.Oracle;
    using FizzCode.DbTools.SqlExecuter.SqLite3;

    public static class SqlExecuterFactory
    {
        public static SqlStatementExecuter CreateSqlExecuter(NamedConnectionString connectionString, ISqlGenerator sqlGenerator)
        {
            var sqlEngineVersion = connectionString.GetSqlEngineVersion();

            if (sqlEngineVersion == SqLiteVersion.SqLite3)
                return new SqLite3Executer(connectionString, sqlGenerator);

            if (sqlEngineVersion == OracleVersion.Oracle12c)
                return new Oracle12cExecuter(connectionString, sqlGenerator);

            if (sqlEngineVersion == MsSqlVersion.MsSql2016)
                return new MsSql2016Executer(connectionString, sqlGenerator);

            throw new NotImplementedException($"Not implemented {sqlEngineVersion}.");
        }

        public static SqlStatementExecuter CreateSqlExecuter(NamedConnectionString connectionString, Context context)
        {
            var sqlEngineVersion = connectionString.GetSqlEngineVersion();

            var generator = SqlGeneratorFactory.CreateSqlGenerator(sqlEngineVersion, context);

            return CreateSqlExecuter(connectionString, generator);
        }
    }
}