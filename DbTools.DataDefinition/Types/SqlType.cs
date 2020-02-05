namespace FizzCode.DbTools.DataDefinition
{
    public class SqlType
    {
        public SqlTypeInfo SqlTypeInfo { get; set; }
        public bool IsNullable { get; set; }
        public int? Length { get; set; }
        public int? Scale { get; set; }

        public SqlType Create(SqlTypeInfo sqlTypeInfo)
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
            var nullable = IsNullable ? " NULL" : " NOT NULL";
            return $"{SqlTypeInfo} ({Length}, {Scale}){nullable}";
        }
    }
}