using FizzCode.DbTools.Common;
using FizzCode.DbTools.Factory.Interfaces;
using FizzCode.DbTools.SqlExecuter;
using FizzCode.LightWeight;

namespace FizzCode.DbTools.DataDefinition.Factory;
public class DatabaseCreatorFactory
{
    private readonly ISqlExecuterFactory _sqlExecuterFactory;
    public DatabaseCreatorFactory(ISqlExecuterFactory sqlExecuterFactory)
    {
        _sqlExecuterFactory = sqlExecuterFactory;

    }

    public DatabaseCreator FromConnectionStringSettings(DatabaseDefinition databaseDefinition, NamedConnectionString connectionString, ContextWithLogger context)
    {
        var executer = _sqlExecuterFactory.CreateSqlExecuter(connectionString);
        return new DatabaseCreator(databaseDefinition, executer);
    }
}