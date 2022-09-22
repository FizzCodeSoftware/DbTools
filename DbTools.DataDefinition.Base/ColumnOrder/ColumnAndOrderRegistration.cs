namespace FizzCode.DbTools.DataDefinition.Base
{
    public class ColumnAndOrderRegistration : ColumnAndOrder
    {
        public string ColumnName { get; set; }

        public ColumnAndOrderRegistration(string columnName, AscDesc order)
            : base(order)
        {
            ColumnName = columnName;
        }

        public override string ToString()
        {
            return $"{ColumnName} {OrderAsKeyword}";
        }
    }
}
