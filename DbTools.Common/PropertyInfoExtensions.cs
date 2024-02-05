using System.Reflection;
using FizzCode.DbTools.Common;

namespace FizzCode.DbTools.DataDeclaration;

public static class PropertyInfoExtensions
{
    public static T GetValueSafe<T>(this PropertyInfo pi, object obj)
    {
        var result = (T?)pi.GetValue(obj);
        Throw.InvalidOperationExceptionIfNull(result, typeof(T), obj.GetType());

        return result!;
    }
}
