namespace FizzCode.DbTools.DataDefinition
{
    public class ForeignKeyGroup
    {
        public string ColumnName { get; }
        public string ReferredColumnName { get; }

        public ForeignKeyGroup(string name, string referredColumnName)
        {
            ColumnName = name;
            ReferredColumnName = referredColumnName;
        }
    }
}