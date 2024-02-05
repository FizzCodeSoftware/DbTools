using System.Globalization;
using System;
using System.Linq;

namespace FizzCode.DbTools.Common;

public static class TypeHelper
{
    public static string GetFriendlyTypeName(this Type? type, bool includeNameSpace = false)
    {
        if (type == null)
            return "<unknown type>";

        if (type.IsArray)
            return GetFriendlyTypeName(type.GetElementType()) + "[]";

        if (type.IsGenericType)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}<{1}>",
                type.Name[..type.Name.LastIndexOf("`", StringComparison.InvariantCultureIgnoreCase)],
                string.Join(", ", type.GetGenericArguments().Select(x => x.GetFriendlyTypeName(false))));
        }

        return type.Name;
    }
}
