namespace FizzCode.DbTools.DataDefinition.SqLite3
{
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition.SqlGenerator;

    public class SqLite3MigrationGenerator : AbstractSqlMigrationGenerator
    {
        public SqLite3MigrationGenerator(Context context)
            : base(context, new SqLite3Generator(context))
        {
        }
    }
}