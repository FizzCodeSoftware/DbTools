using FizzCode.DbTools.Common;
using FizzCode.DbTools.DataDefinition.SqlGenerator;

namespace FizzCode.DbTools.DataDefinition.MsSql2016;
public class MsSql2016MigrationGenerator : AbstractSqlMigrationGenerator
{
    public MsSql2016MigrationGenerator(ContextWithLogger context)
        : base(context, new MsSql2016Generator(context))
    {
    }
}