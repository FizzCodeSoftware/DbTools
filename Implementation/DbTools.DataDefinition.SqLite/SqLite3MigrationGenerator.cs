using FizzCode.DbTools.Common;
using FizzCode.DbTools.DataDefinition.SqlGenerator;

namespace FizzCode.DbTools.DataDefinition.SqLite3;
public class SqLite3MigrationGenerator(ContextWithLogger context)
    : AbstractSqlMigrationGenerator(context, new SqLite3Generator(context))
{
}