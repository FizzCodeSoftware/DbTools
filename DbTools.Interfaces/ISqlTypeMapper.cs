using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.Interfaces;
public interface ISqlTypeMapper
{
    string GetType(SqlType type);
}