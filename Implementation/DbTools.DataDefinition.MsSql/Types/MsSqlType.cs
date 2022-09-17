namespace FizzCode.DbTools.DataDefinition
{
    using FizzCode.DbTools.DataDefinition.Base;

    public abstract class MsSqlType : SqlTypeInfo
    {
        public virtual bool IsMaxLengthAllowed { get; }
    }
}