namespace FizzCode.DbTools.DataDefinitionGenerator
{
    using FizzCode.DbTools.Common;

    public class OracleMigrationGenerator : GenericSqlMigrationGenerator
    {
        public OracleMigrationGenerator(Context context) : base(context)
        {
        }

        protected override ISqlGenerator CreateGenerator()
        {
            return new OracleGenerator12c(Context);
        }
    }
}