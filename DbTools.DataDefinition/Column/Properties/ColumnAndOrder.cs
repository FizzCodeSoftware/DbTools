namespace FizzCode.DbTools.DataDefinition
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

    public class ColumnAndOrder
    {
        public SqlColumnBase SqlColumn { get; set; }
        public AscDesc Order { get; set; }
        public string OrderAsKeyword => Order.ToString().ToUpperInvariant();

        protected ColumnAndOrder(AscDesc order)
        {
            Order = order;
        }

        public ColumnAndOrder(SqlColumnBase sqlColumn, AscDesc order)
            : this(order)
        {
            SqlColumn = sqlColumn;
        }

        public override string ToString()
        {
            return $"{SqlColumn.Name} {OrderAsKeyword}";
        }
    }
}
