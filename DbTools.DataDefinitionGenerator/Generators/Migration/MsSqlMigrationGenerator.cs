namespace FizzCode.DbTools.DataDefinitionGenerator
{
    using FizzCode.DbTools.Common;

    public class MsSqlMigrationGenerator : GenericSqlMigrationGenerator
    {
        public MsSqlMigrationGenerator(Context context)
            : base(context)
        {
        }

        protected override ISqlGenerator CreateGenerator()
        {
            return new MsSqlGenerator2016(Context);
        }
    }
}