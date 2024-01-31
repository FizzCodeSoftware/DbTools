namespace FizzCode.DbTools.Common
{
    using System.Collections.Generic;

    public class RowSet : List<Row>
    {
        public RowSet()
        { }

        public RowSet(IEnumerable<Row> collection) : base(collection)
        { }
    }
}
