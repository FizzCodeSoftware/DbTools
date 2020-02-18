namespace FizzCode.DbTools.DataDefinitionGenerator
{
    using FizzCode.DbTools.Common;

    public class SqLiteMigrationGenerator : GenericSqlMigrationGenerator
    {
        public SqLiteMigrationGenerator(Context context)
            : base(context)
        {
        }

        protected override ISqlGenerator CreateGenerator()
        {
            return new SqLiteGenerator3(Context);
        }
    }
}