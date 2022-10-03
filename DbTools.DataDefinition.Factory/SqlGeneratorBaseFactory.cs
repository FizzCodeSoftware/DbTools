namespace FizzCode.DbTools.DataDefinition.Factory
{
    using System;
    using FizzCode.DbTools.Factory.Interfaces;
    using FizzCode.DbTools.Interfaces;
    using FizzCode.DbTools.SqlGenerator.MsSql;
    using FizzCode.DbTools.SqlGenerator.Oracle;
    using FizzCode.DbTools.SqlGenerator.SqLite;

    public class SqlGeneratorBaseFactory : ISqlGeneratorBaseFactory
    {
        protected IContextFactory ContextFactory { get; }
        public SqlGeneratorBaseFactory(IContextFactory contextFactory)
        {
            ContextFactory = contextFactory;
        }

        public ISqlGeneratorBase CreateGenerator(SqlEngineVersion version)
        {
            var context = ContextFactory.CreateContext(version);

            if (version == SqLiteVersion.SqLite3)
                return new SqLiteGenerator(context);

            if (version == MsSqlVersion.MsSql2016)
                return new MsSqlGenerator(context);

            if (version == OracleVersion.Oracle12c)
                return new OracleGenerator(context);

            throw new NotImplementedException($"Not implemented {version}.");
        }
    }
}