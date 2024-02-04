namespace FizzCode.DbTools.DataDefinition.Base.Interfaces;

public interface ISqlTypeInfo
{
    bool Deprecated { get; }
    bool HasLength { get; }
    bool HasScale { get; }
    string SqlDataType { get; }

    string ToString();
}