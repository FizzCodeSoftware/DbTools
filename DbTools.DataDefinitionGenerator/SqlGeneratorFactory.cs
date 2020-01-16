namespace FizzCode.DbTools.DataDefinitionGenerator
{
    using System;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;

    public static class SqlGeneratorFactory
    {
        public static ISqlGenerator CreateGenerator(SqlVersion version, Context context)
        {
            if (version is ISqlDialect)
                return new SqLiteGenerator(context);

            if (version is IMsSqlDialect)
                return new MsSqlGenerator(context);

            if (version is IOracleDialect)
                return new OracleGenerator(context);

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