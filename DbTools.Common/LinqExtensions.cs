using System.Collections.Generic;
using System.Linq;

namespace FizzCode.DbTools.Common;

public static class LinqExtensions
{
    public static IEnumerable<TSource> WhereNotNull<TSource>(this IEnumerable<TSource?> source)
    {
        return source.Where(s => s is not null)!;
    }
}
