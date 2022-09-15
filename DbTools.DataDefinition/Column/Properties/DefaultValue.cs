namespace FizzCode.DbTools.DataDefinition
{
    using FizzCode.DbTools.DataDefinition.Base;

    public class DefaultValue : SqlColumnProperty
    {
        public string Value { get; }

        public DefaultValue(SqlColumn sqlColumn, string value)
            : base(sqlColumn)
        {
            Value = value;
        }
    }
}