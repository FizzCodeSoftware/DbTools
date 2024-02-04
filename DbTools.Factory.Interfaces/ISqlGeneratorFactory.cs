using FizzCode.DbTools.Interfaces;

namespace FizzCode.DbTools.Factory.Interfaces;
public interface ISqlGeneratorFactory
{
    ISqlGenerator CreateSqlGenerator(SqlEngineVersion version);
}