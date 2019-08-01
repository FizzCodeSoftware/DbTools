namespace FizzCode.DbTools.DataDefinition
{
    public class Identity : SqlColumnProperty
    {
        public int Increment { get; set; } = 1;
        public int Seed { get; set; } = 1;

        public Identity(SqlColumn sqlColumn)
            : base(sqlColumn)
        {
        }
    }
}