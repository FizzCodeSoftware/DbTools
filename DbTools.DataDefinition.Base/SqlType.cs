namespace FizzCode.DbTools.DataDefinition.Base
{
    using System.Globalization;
    using System.Text;
    using FizzCode.DbTools.DataDefinition.Base.Interfaces;

    public class SqlType : ISqlType
    {
        public ISqlTypeInfo SqlTypeInfo { get; set; }
        public bool IsNullable { get; set; }
        public int? Length { get; set; }
        public int? Scale { get; set; }

        public ISqlType Clone(ISqlTypeInfo sqlTypeInfo)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = sqlTypeInfo,
                IsNullable = IsNullable,
                Length = Length,
                Scale = Scale
            };

            return sqlType;
        }

        public ISqlType Copy()
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = SqlTypeInfo,
                IsNullable = IsNullable,
                Length = Length,
                Scale = Scale
            };

            return sqlType;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(SqlTypeInfo.ToString());
            if (Length != null || Scale != null)
            {
                sb.Append(" (")
                    .Append(Length?.ToString("D", CultureInfo.InvariantCulture));
                if (Scale != null)
                    sb.Append(Scale?.ToString("D", CultureInfo.InvariantCulture));

                sb.Append(')');
            }

            if (!IsNullable)
                sb.Append(" NOT");

            sb.Append(" NULL");

            return sb.ToString();
        }
    }
}