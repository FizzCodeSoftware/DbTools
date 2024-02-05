using FizzCode.DbTools.Interfaces;
using FizzCode.LightWeight;

namespace FizzCode.DbTools.Factory.Interfaces;
public interface ISqlExecuterFactory
{
    ISqlStatementExecuter CreateSqlExecuter(NamedConnectionString connectionString);
}