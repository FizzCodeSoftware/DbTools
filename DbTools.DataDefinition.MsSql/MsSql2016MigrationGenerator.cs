namespace FizzCode.DbTools.DataDefinition.MsSql2016
{
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition.SqlGenerator;

    public class MsSql2016MigrationGenerator : AbstractSqlMigrationGenerator
    {
        public MsSql2016MigrationGenerator(Context context)
            : base(context, new MsSql2016Generator(context))
        {
        }
    }
}