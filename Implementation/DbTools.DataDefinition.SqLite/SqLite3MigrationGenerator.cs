using FizzCode.DbTools.Common;
using FizzCode.DbTools.DataDefinition.SqlGenerator;

namespace FizzCode.DbTools.DataDefinition.SqLite3;
public class SqLite3MigrationGenerator : AbstractSqlMigrationGenerator
{
    public SqLite3MigrationGenerator(ContextWithLogger context)
        : base(context, new SqLite3Generator(context))
    {
    }
}