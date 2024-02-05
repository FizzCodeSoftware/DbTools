using System.Reflection;

namespace FizzCode.DbTools.Common;

public static class PropertyInfoExtensions
{
    public static T GetValueSafe<T>(this PropertyInfo pi, object obj)
    {
        var result = (T?)pi.GetValue(obj);
        Throw.InvalidOperationExceptionIfNull(result, typeof(T), obj.GetType());

        return result!;
    }
}
