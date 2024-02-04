using FizzCode.DbTools.Interfaces;
using FizzCode.LightWeight.AdoNet;

namespace FizzCode.DbTools.Factory.Interfaces;
public interface ISqlExecuterFactory
{
    ISqlStatementExecuter CreateSqlExecuter(NamedConnectionString connectionString);
}