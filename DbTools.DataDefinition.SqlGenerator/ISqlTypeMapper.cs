namespace FizzCode.DbTools.DataDefinition.SqlGenerator
{
    using FizzCode.DbTools.DataDefinition.Base;

    public interface ISqlTypeMapper
    {
        string GetType(SqlType type);
    }
}