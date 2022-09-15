
namespace FizzCode.DbTools.DataDefinition
{
    using FizzCode.DbTools.DataDefinition.Base;
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
}