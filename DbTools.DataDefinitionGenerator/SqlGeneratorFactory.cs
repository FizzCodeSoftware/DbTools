namespace FizzCode.DbTools.DataDefinitionGenerator
{
    using System;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;

    public static class SqlGeneratorFactory
    {
        public static ISqlGenerator CreateGenerator(SqlVersion version, Context context)
        {
            if (version is SqLite3)
                return new SqLiteGenerator3(context);

            if (version is MsSql2016)
                return new MsSqlGenerator2016(context);

            if (version is Oracle12c)
                return new OracleGenerator12c(context);

            throw new NotImplementedException($"Not implemented {version}.");
        }

        public static ISqlMigrationGenerator CreateMigrationGenerator(SqlVersion version, Context context)
        {
            if (version is SqLite3)
                return new SqLiteMigrationGenerator(context);

            if (version is MsSql2016)
                return new MsSqlMigrationGenerator(context);

            if (version is Oracle12c)
                return new OracleMigrationGenerator(context);

            throw new NotImplementedException($"Not implemented {version}.");
        }
    }
}