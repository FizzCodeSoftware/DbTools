using System.Collections.Generic;

namespace FizzCode.DbTools.Common;
public static class RowSetExtensions
{
    public static RowSet ToRowSet(this IEnumerable<Row> source)
    {
        if (source == null)
            throw new System.ArgumentNullException(nameof(source));

        return new RowSet(source);
    }
}
