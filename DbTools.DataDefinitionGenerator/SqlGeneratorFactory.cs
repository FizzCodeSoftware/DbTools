namespace FizzCode.DbTools.DataDefinitionGenerator
{
    using System;
    using FizzCode.DbTools.Common;

    public static class SqlGeneratorFactory
    {
        public static ISqlGenerator CreateGenerator(SqlDialect dialect, GeneratorContext context)
        {
            return dialect switch
            {
                SqlDialect.SqLite => new SqLiteGenerator(context),
                SqlDialect.MsSql => new MsSqlGenerator(context),
                SqlDialect.Oracle => new OracleGenerator(context),
                _ => throw new NotImplementedException($"Not implemented {dialect}."),
            };
        }
    }
}