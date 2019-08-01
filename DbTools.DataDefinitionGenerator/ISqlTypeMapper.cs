namespace FizzCode.DbTools.DataDefinitionGenerator
{
    using FizzCode.DbTools.DataDefinition;

    public interface ISqlTypeMapper
    {
        string GetType(SqlType type);
    }
}