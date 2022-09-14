namespace FizzCode.DbTools.DataDefinition
{
    public abstract class SqlColumnProperty
    {
        public SqlColumnBase SqlColumn { get; }

        protected SqlColumnProperty(SqlColumnBase sqlColumn)
        {
            SqlColumn = sqlColumn;
        }
    }
}