namespace FizzCode.DbTools.DataDefinition
{
    public abstract class TypeMapper
    {
        protected virtual SqlType MapSqlType(SqlTypeInfo sqlTypeInfo, bool isNullable, int length = 0, int scale = 0)
        {
            var sqlType = new SqlType
            {
                Length = length,
                Scale = scale,
                IsNullable = isNullable,
                SqlTypeInfo = sqlTypeInfo
            };

            return sqlType;
        }

        public abstract SqlType MapFromGeneric1(SqlType genericType);
    }
}