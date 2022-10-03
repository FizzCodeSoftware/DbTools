namespace FizzCode.DbTools.DataDefinition.Factory
{
    using System;
    using FizzCode.DbTools.DataDefinition.MsSql2016;
    using FizzCode.DbTools.DataDefinition.Oracle12c;
    using FizzCode.DbTools.DataDefinition.SqLite3;
    using FizzCode.DbTools.Factory.Interfaces;
    using FizzCode.DbTools.Interfaces;

    public class SqlGeneratorFactory : ISqlGeneratorFactory
    {
        private readonly IContextFactory _contextFactory;

        public SqlGeneratorFactory(IContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public ISqlGenerator CreateSqlGenerator(SqlEngineVersion version)
        {
            var context = _contextFactory.CreateContext(version);

            if (version == SqLiteVersion.SqLite3)
                return new SqLite3Generator(context);

            if (version == MsSqlVersion.MsSql2016)
                return new MsSql2016Generator(context);

            if (version == OracleVersion.Oracle12c)
                return new Oracle12cGenerator(context);

            throw new NotImplementedException($"Not implemented {version}.");
        }

        
    }
}