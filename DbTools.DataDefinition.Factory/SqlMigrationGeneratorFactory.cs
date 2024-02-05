using System;
using FizzCode.DbTools.DataDefinition.MsSql2016;
using FizzCode.DbTools.DataDefinition.Oracle12c;
using FizzCode.DbTools.DataDefinition.SqLite3;
using FizzCode.DbTools.Factory.Interfaces;
using FizzCode.DbTools.Interfaces;

namespace FizzCode.DbTools.DataDefinition.Factory;
public class SqlMigrationGeneratorFactory(IContextFactory contextFactory) : ISqlMigrationGeneratorFactory
{
    protected readonly IContextFactory _contextFactory = contextFactory;

    public ISqlMigrationGenerator CreateMigrationGenerator(SqlEngineVersion version)
    {
        var context = _contextFactory.CreateContextWithLogger(version);

        if (version == SqLiteVersion.SqLite3)
            return new SqLite3MigrationGenerator(context);

        if (version == MsSqlVersion.MsSql2016)
            return new MsSql2016MigrationGenerator(context);

        if (version == OracleVersion.Oracle12c)
            return new Oracle12cMigrationGenerator(context);

        throw new NotImplementedException($"Not implemented {version}.");
    }
}