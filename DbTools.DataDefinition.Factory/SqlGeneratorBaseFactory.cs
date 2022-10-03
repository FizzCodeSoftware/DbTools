namespace FizzCode.DbTools.DataDefinition.Factory
{
    using System;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Factory.Interfaces;
    using FizzCode.DbTools.Interfaces;
    using FizzCode.DbTools.SqlGenerator.MsSql;
    using FizzCode.DbTools.SqlGenerator.Oracle;
    using FizzCode.DbTools.SqlGenerator.SqLite;

    public class SqlGeneratorBaseFactory : ISqlGeneratorBaseFactory
    {
        protected Context Context { get; }
        public SqlGeneratorBaseFactory(Context context)
        {
            Context = context;
        }

        public ISqlGeneratorBase CreateGenerator(SqlEngineVersion version)
        {
            if (version == SqLiteVersion.SqLite3)
                return new SqLiteGenerator(Context);

            if (version == MsSqlVersion.MsSql2016)
                return new MsSqlGenerator(Context);

            if (version == OracleVersion.Oracle12c)
                return new OracleGenerator(Context);

            throw new NotImplementedException($"Not implemented {version}.");
        }
    }
}