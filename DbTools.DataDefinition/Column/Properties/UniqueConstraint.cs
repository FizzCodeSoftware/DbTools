namespace FizzCode.DbTools.DataDefinition
{
    public class UniqueConstraint : IndexBase
    {
        public UniqueConstraint(SqlTable sqlTable, string name)
            : base(sqlTable, name, true)
        {
        }
    }
}
