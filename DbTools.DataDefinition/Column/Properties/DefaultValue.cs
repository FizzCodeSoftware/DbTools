namespace FizzCode.DbTools.DataDefinition
{
    public class DefaultValue : SqlColumnProperty
    {
        public object Value { get; }

        public DefaultValue(SqlColumn sqlColumn, object value)
            : base(sqlColumn)
        {
            Value = value;
        }
    }
}