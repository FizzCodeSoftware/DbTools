namespace FizzCode.DbTools.DataDefinition
{
    public class SqlType
    {
        public SqlTypeInfo SqlTypeInfo { get; set; }
        public bool IsNullable { get; set; }
        public int? Length { get; set; }
        public int? Scale { get; set; }

        public SqlType CopyTo(SqlType sqltype)
        {
            sqltype.SqlTypeInfo = SqlTypeInfo;
            sqltype.IsNullable = IsNullable;
            sqltype.Length = Length;
            sqltype.Scale = Scale;
            return sqltype;
        }
    }
}