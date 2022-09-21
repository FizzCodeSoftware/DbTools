namespace FizzCode.DbTools.DataDefinition.Factory
{
    using System;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition.MsSql2016;
    using FizzCode.DbTools.DataDefinition.Oracle12c;
    using FizzCode.DbTools.DataDefinition.SqlGenerator;
    using FizzCode.DbTools.DataDefinition.SqLite3;
    using FizzCode.DbTools.Factory.Interfaces;

    public class SqlGeneratorFactory : ISqlGeneratorFactory
    {
        public ISqlGenerator CreateSqlGenerator(SqlEngineVersion version, Context context)
        {
            if (version == SqLiteVersion.SqLite3)
                return new SqLite3Generator(context);

            if (version == MsSqlVersion.MsSql2016)
                return new MsSql2016Generator(context);

            if (version == OracleVersion.Oracle12c)
                return new Oracle12cGenerator(context);

            throw new NotImplementedException($"Not implemented {version}.");
        }

        public ISqlMigrationGenerator CreateMigrationGenerator(SqlEngineVersion version, Context context)
        {
            if (version == SqLiteVersion.SqLite3)
                return new SqLite3MigrationGenerator(context);

            if (version == MsSqlVersion.MsSql2016)
                return new MsSql2016MigrationGenerator(context);

            if (version == OracleVersion.Oracle12c)
                return new Oracle12cMigrationGenerator(context);

            throw new NotImplementedException($"Not implemented {version}.");
        }
    }
}