namespace FizzCode.DbTools.DataDefinition.Base
{
    public abstract class AbstractTypeMapper : ITypeMapper
    {
        public abstract SqlEngineVersion SqlVersion { get; }

        protected virtual SqlType MapSqlType(SqlTypeInfo sqlTypeInfo, bool isNullable, int? length = null, int? scale = null)
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