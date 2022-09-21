namespace FizzCode.DbTools.Factory.Interfaces
{
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.SqlExecuter;
    using FizzCode.LightWeight.AdoNet;

    public interface ISqlMigratorFactory
    {
        DatabaseMigrator FromConnectionStringSettings(NamedConnectionString connectionString, Context context);
    }
}