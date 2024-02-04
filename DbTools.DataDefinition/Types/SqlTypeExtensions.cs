using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDefinition;
public static class SqlTypeExtensions
{
    public static bool AnyOf(this SqlType sqlType, params SqlTypeInfo[] sqlTypeInfos)
    {
        foreach (var sqlTypeInfo in sqlTypeInfos)
        {
            if (sqlType.SqlTypeInfo.GetType().Name == sqlTypeInfo.GetType().Name)
                return true;
        }

        return false;
    }
}