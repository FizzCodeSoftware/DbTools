namespace FizzCode.DbTools.DataDefinitionGenerator
{
    using System;
    using FizzCode.DbTools.DataDefinition;

    public static class SqlGeneratorFactory
    {
        public static ISqlGenerator CreateGenerator(SqlDialect dialect)
        {
            switch (dialect)
            {
                case SqlDialect.SqLite:
                    return new SqLiteGenerator();
                case SqlDialect.MsSql:
                    return new MsSqlGenerator();
                case SqlDialect.Oracle:
                    return new OracleGenerator();
                default:
                    throw new NotImplementedException($"Not implemented {dialect}.");
            }
        }
    }
}