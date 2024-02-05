using System.Collections.Generic;

namespace FizzCode.DbTools.Common;
public static class RowSetExtensions
{
    public static RowSet ToRowSet(this IEnumerable<Row> source)
    {
        System.ArgumentNullException.ThrowIfNull(source);

        return new RowSet(source);
    }
}
