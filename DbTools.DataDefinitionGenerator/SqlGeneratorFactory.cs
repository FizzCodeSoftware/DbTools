namespace FizzCode.DbTools.DataDefinitionGenerator
{
    using System;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;

    public static class SqlGeneratorFactory
    {
        public static ISqlGenerator CreateGenerator(SqlVersion version, Context context)
        {
            if (version is ISqLiteDialect)
                return new SqLiteGenerator3(context);

            if (version is IMsSqlDialect)
                return new MsSqlGenerator2016(context);

            if (version is IOracleDialect)
                return new OracleGenerator12c(context);

            throw new NotImplementedException($"Not implemented {version}.");
        }

        public static ISqlMigrationGenerator CreateMigrationGenerator(SqlVersion version, Context context)
        {
            if (version is ISqlDialect)
                return new SqLiteMigrationGenerator(context);

            if (version is IMsSqlDialect)
                return new MsSqlMigrationGenerator(context);

            if (version is IOracleDialect)
                return new OracleMigrationGenerator(context);

            throw new NotImplementedException($"Not implemented {version}.");
        }
    }
}