using FizzCode.DbTools.Interfaces;

namespace FizzCode.DbTools.Factory.Interfaces;
public interface ISqlGeneratorBaseFactory
{
    ISqlGeneratorBase CreateGenerator(SqlEngineVersion version);
}