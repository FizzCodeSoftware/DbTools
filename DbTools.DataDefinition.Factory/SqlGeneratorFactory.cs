namespace FizzCode.DbTools.DataDefinition
{
    using System;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition.MsSql2016;
    using FizzCode.DbTools.DataDefinition.Oracle12c;
    using FizzCode.DbTools.DataDefinition.SqlGenerator;
    using FizzCode.DbTools.DataDefinition.SqLite3;
    using FizzCode.DbTools.SqlGenerator.Base;
    using FizzCode.DbTools.SqlGenerator.MsSql;
    using FizzCode.DbTools.SqlGenerator.Oracle;
    using FizzCode.DbTools.SqlGenerator.SqLite;

    public static class SqlGeneratorFactory
    {
        public static ISqlGeneratorBase CreateGenerator(SqlEngineVersion version, Context context)
        {
            if (version == SqLiteVersion.SqLite3)
                return new SqLiteGenerator(context);

            if (version == MsSqlVersion.MsSql2016)
                return new MsSqlGenerator(context);

            if (version == OracleVersion.Oracle12c)
                return new OracleGenerator(context);

            throw new NotImplementedException($"Not implemented {version}.");
        }

        public static ISqlGenerator CreateSqlGenerator(SqlEngineVersion version, Context context)
        {
            if (version == SqLiteVersion.SqLite3)
                return new SqLite3Generator(context);

            if (version == MsSqlVersion.MsSql2016)
                return new MsSql2016Generator(context);

            if (version == OracleVersion.Oracle12c)
                return new Oracle12cGenerator(context);

            throw new NotImplementedException($"Not implemented {version}.");
        }

        public static ISqlMigrationGenerator CreateMigrationGenerator(SqlEngineVersion version, Context context)
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