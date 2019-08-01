namespace FizzCode.DbTools.DataDefinition
{
    public class PrimaryKey : IndexBase
    {
        public PrimaryKey(SqlTable sqlTable, string name)
            : base(sqlTable, name)
        {
        }
    }
}
