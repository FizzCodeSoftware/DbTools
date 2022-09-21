namespace FizzCode.DbTools.DataDefinition.Base
{
    using FizzCode.DbTools;

    public interface ITypeMapper
    {
        SqlEngineVersion SqlVersion { get; }

        SqlType MapFromGeneric1(SqlType genericType);
        SqlType MapToGeneric1(SqlType sqlType);
    }
}