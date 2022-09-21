namespace FizzCode.DbTools.Factory.Interfaces
{
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.SqlExecuter;
    using FizzCode.LightWeight.AdoNet;

    public interface ISqlExecuterFactory
    {
        ISqlStatementExecuter CreateSqlExecuter(NamedConnectionString connectionString, Context context);
    }
}