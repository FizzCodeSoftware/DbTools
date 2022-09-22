namespace FizzCode.DbTools.DataDefinition.Base
{
    using FizzCode.DbTools.DataDefinition.Base.Interfaces;

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

        public abstract ISqlType MapFromGeneric1(ISqlType genericType);

        public abstract ISqlType MapToGeneric1(ISqlType sqlType);
    }
}