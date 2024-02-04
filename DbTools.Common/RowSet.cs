using System.Collections.Generic;

namespace FizzCode.DbTools.Common;
public class RowSet : List<Row>
{
    public RowSet()
    { }

    public RowSet(IEnumerable<Row> collection) : base(collection)
    { }
}
