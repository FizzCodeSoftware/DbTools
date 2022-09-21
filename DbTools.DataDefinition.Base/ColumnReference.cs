namespace FizzCode.DbTools.DataDefinition.Base
{
    public class ColumnReference
    {
        public string ColumnName { get; }
        public string ReferredColumnName { get; }

        public ColumnReference(string name, string referredColumn)
        {
            ColumnName = name;
            ReferredColumnName = referredColumn;
        }
    }
}