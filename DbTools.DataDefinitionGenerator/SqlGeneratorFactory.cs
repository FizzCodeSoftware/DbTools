namespace FizzCode.DbTools.DataDefinitionGenerator
{
    using System;
    using FizzCode.DbTools.Common;

    public static class SqlGeneratorFactory
    {
        public static ISqlGenerator CreateGenerator(SqlDialect dialect, Settings settings)
        {
            switch (dialect)
            {
                case SqlDialect.SqLite:
                    return new SqLiteGenerator(settings);
                case SqlDialect.MsSql:
                    return new MsSqlGenerator(settings);
                case SqlDialect.Oracle:
                    return new OracleGenerator(settings);
                default:
                    throw new NotImplementedException($"Not implemented {dialect}.");
            }
        }
    }
}