namespace FizzCode.DbTools.DataDefinition.Factory
{
    using System;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Factory.Interfaces;
    using FizzCode.DbTools.SqlGenerator.Base;
    using FizzCode.DbTools.SqlGenerator.MsSql;
    using FizzCode.DbTools.SqlGenerator.Oracle;
    using FizzCode.DbTools.SqlGenerator.SqLite;

    public class SqlGeneratorBaseFactory : ISqlGeneratorBaseFactory
    {
        public ISqlGeneratorBase CreateGenerator(SqlEngineVersion version, Context context)
        {
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