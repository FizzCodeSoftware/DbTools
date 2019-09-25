namespace FizzCode.DbTools.DataDefinitionGenerator
{
    using System;
    using FizzCode.DbTools.Common;

    public static class SqlGeneratorFactory
    {
        public static ISqlGenerator CreateGenerator(SqlDialect dialect, Settings settings)
        {
            return dialect switch
            {
                SqlDialect.SqLite => new SqLiteGenerator(settings),
                SqlDialect.MsSql => new MsSqlGenerator(settings),
                SqlDialect.Oracle => new OracleGenerator(settings),
                _ => throw new NotImplementedException($"Not implemented {dialect}."),
            };
        }
    }
}