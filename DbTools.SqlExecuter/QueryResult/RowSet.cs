namespace FizzCode.DbTools.SqlExecuter
{
    using System.Collections.Generic;
    using FizzCode.DbTools.Common;

    public class RowSet
    {
        public List<Row> Rows { get; } = new List<Row>();
    }
}
