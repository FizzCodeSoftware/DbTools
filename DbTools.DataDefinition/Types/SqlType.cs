namespace FizzCode.DbTools.DataDefinition
{
    using System.Globalization;
    using System.Text;

    public class SqlType
    {
        public SqlTypeInfo SqlTypeInfo { get; set; }
        public bool IsNullable { get; set; }
        public int? Length { get; set; }
        public int? Scale { get; set; }

        public SqlType Clone(SqlTypeInfo sqlTypeInfo)
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

        public SqlType Copy()
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

                sb.Append(")");
            }

            if (!IsNullable)
                sb.Append(" NOT");

            sb.Append(" NULL");

            return sb.ToString();
        }
    }
}