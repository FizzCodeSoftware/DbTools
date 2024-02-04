namespace FizzCode.DbTools.DataDefinition.Base.Interfaces;
public interface ITypeMapper
{
    SqlEngineVersion SqlVersion { get; }

    ISqlType MapFromGeneric1(ISqlType genericType);
    ISqlType MapToGeneric1(ISqlType sqlType);
}