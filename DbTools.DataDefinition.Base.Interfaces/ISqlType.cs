namespace FizzCode.DbTools.DataDefinition.Base.Interfaces
{
    public interface ISqlType
    {
        bool IsNullable { get; set; }
        int? Length { get; set; }
        int? Scale { get; set; }
        ISqlTypeInfo SqlTypeInfo { get; set; }

        ISqlType Clone(ISqlTypeInfo sqlTypeInfo);
        ISqlType Copy();
        string ToString();
    }
}