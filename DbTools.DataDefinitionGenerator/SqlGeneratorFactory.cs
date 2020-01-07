namespace FizzCode.DbTools.DataDefinitionGenerator
{
    using System;
    using FizzCode.DbTools.Common;

    public static class SqlGeneratorFactory
    {
        public static ISqlGenerator CreateGenerator(SqlDialect dialect, Context context)
        {
            return dialect switch
            {
                SqlDialect.SqLite => new SqLiteGenerator(context),
                SqlDialect.MsSql => new MsSqlGenerator(context),
                SqlDialect.Oracle => new OracleGenerator(context),
                _ => throw new NotImplementedException($"Not implemented {dialect}."),
            };
        }

        public static ISqlMigrationGenerator CreateMigrationGenerator(SqlDialect dialect, Context context)
        {
            return dialect switch
            {
                SqlDialect.SqLite => new SqLiteMigrationGenerator(context),
                SqlDialect.MsSql => new MsSqlMigrationGenerator(context),
                SqlDialect.Oracle => new OracleMigrationGenerator(context),
                _ => throw new NotImplementedException($"Not implemented {dialect}."),
            };
        }
    }
}