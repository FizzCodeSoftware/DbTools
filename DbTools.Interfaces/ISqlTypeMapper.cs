namespace FizzCode.DbTools.Interfaces
{
    using FizzCode.DbTools.DataDefinition.Base;

    public interface ISqlTypeMapper
    {
        string GetType(SqlType type);
    }
}