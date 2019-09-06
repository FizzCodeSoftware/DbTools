namespace FizzCode.DbTools.DataDefinition
{
    public class ForeignKeyGroup
    {
        public string ColumnName { get; }
        public string ReferredColumn { get; }

        public ForeignKeyGroup(string name, string referredColumn)
        {
            ColumnName = name;
            ReferredColumn = referredColumn;
        }
    }
}