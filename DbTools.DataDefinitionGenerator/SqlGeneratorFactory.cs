namespace FizzCode.DbTools.DataDefinitionGenerator
{
    using System;
    using FizzCode.DbTools.Common;

    public static class SqlGeneratorFactory
    {
        public static ISqlGenerator CreateGenerator(SqlVersion version, Context context)
        {
            return version.SqlDialect switch
            {
                SqlDialectX.SqLite => new SqLiteGenerator(context),
                SqlDialectX.MsSql => new MsSqlGenerator(context),
                SqlDialectX.Oracle => new OracleGenerator(context),
                _ => throw new NotImplementedException($"Not implemented {version.SqlDialect}."),
            };
        }

        public static ISqlMigrationGenerator CreateMigrationGenerator(SqlVersion version, Context context)
        {
            return version.SqlDialect switch
            {
                SqlDialectX.SqLite => new SqLiteMigrationGenerator(context),
                SqlDialectX.MsSql => new MsSqlMigrationGenerator(context),
                SqlDialectX.Oracle => new OracleMigrationGenerator(context),
                _ => throw new NotImplementedException($"Not implemented {version.SqlDialect}."),
            };
        }
    }
}