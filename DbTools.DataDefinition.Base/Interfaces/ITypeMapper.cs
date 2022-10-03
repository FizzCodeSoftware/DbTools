namespace FizzCode.DbTools.DataDefinition.Base.Interfaces
{
    using FizzCode.DbTools;

    public interface ITypeMapper
    {
        SqlEngineVersion SqlVersion { get; }

        ISqlType MapFromGeneric1(ISqlType genericType);
        ISqlType MapToGeneric1(ISqlType sqlType);
    }
}