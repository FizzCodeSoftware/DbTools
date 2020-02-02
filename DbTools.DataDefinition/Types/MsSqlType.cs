namespace FizzCode.DbTools.DataDefinition
{

    public abstract class MsSqlType : SqlTypeInfo
    {
        public virtual bool IsMaxLengthAllowed { get; }
    }
}