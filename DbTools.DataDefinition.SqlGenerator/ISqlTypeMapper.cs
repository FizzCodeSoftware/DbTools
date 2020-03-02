namespace FizzCode.DbTools.DataDefinition.SqlGenerator
{
    using FizzCode.DbTools.DataDefinition;

    public interface ISqlTypeMapper
    {
        string GetType(SqlType type);
    }
}