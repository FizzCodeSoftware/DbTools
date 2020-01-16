namespace FizzCode.DbTools.DataDefinition
{
    using System;

    public abstract class TypeMapper
    {
        public SqlTypeInfos SqlTypeInfos { get; protected set; }

        protected virtual SqlType MapSqlType(string type, bool isNullable, int length = 0, int scale = 0)
        {
            var typeName = type.ToUpper();
            SqlTypeInfos.TryGetValue(typeName, out var sqlTypeInfo);

            if (sqlTypeInfo == null)
                throw new NotImplementedException($"Unmapped SqlType: {type}.");

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

        protected SqlType MapAs(string dbType, SqlType genericType)
        {
            var result = new SqlType();
            genericType.CopyTo(result);
            result.SqlTypeInfo = SqlTypeInfos[dbType];
            return result;
        }

        protected abstract SqlTypeInfos GetTypeInfos();
    }
}