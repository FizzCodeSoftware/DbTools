namespace FizzCode.DbTools.DataDefinition
{
    public abstract class SqlTableCustomProperty : SqlTableProperty
    {
        protected SqlTableCustomProperty()
            : base(null)
        {
        }

        protected SqlTableCustomProperty(SqlTable sqlTable)
            : base(sqlTable)
        {
        }
    }
}
