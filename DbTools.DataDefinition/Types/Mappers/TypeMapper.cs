namespace FizzCode.DbTools.DataDefinition
{
    using FizzCode.DbTools.Configuration;

    public abstract class TypeMapper
    {
        public abstract SqlVersion SqlVersion { get; }

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

        public abstract SqlType MapToGeneric1(SqlType sqlType);
    }
}