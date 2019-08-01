namespace FizzCode.DbTools.DataDefinition
{
    public class ColumnAndOrder
    {
        public SqlColumn SqlColumn { get; set; }
        public AscDesc Order { get; set; }
        public string OrderAsKeyword => Order.ToString().ToUpperInvariant();

        public ColumnAndOrder(SqlColumn sqlColumn, AscDesc order)
        {
            SqlColumn = sqlColumn;
            Order = order;
        }

        public override string ToString()
        {
            return $"{SqlColumn.Name} {OrderAsKeyword}";
        }
    }
}
