namespace FizzCode.DbTools.DataDefinition
{
    using System;

    public class SqLiteTypeMapper3 : TypeMapper
    {
        public SqLiteTypeMapper3()
        {
            SqlTypeInfos = GetTypeInfos();
        }

        public override SqlType MapFromGeneric1(SqlType genericType)
        {
            var result = new SqlType();
            switch (genericType.SqlTypeInfo.DbType)
            {
                case "CHAR":
                case "NCHAR":
                case "VARCHAR":
                case "NVARCHAR":
                case "DATE":
                case "DATETIME":
                    {
                        result.SqlTypeInfo = SqlTypeInfos["TEXT"];
                        return result;
                    }
                case "FLOAT_SMALL":
                case "FLOAT_LARGE":
                    {
                        result.SqlTypeInfo = SqlTypeInfos["REAL"];
                        return result;
                    }
                case "BIT":
                case "BYTE":
                case "INT16":
                case "INT32":
                case "INT64":
                    {
                        result.SqlTypeInfo = SqlTypeInfos["INTEGER"];
                        return result;
                    }
                default:
                    throw new NotImplementedException($"Unmapped type {genericType.SqlTypeInfo.DbType}");
            }
        }

        protected override SqlTypeInfos GetTypeInfos()
        {
            var sqlTypeInfos = new SqlTypeInfos
            {
                new SqlTypeInfo("INTEGER"),
                new SqlTypeInfo("REAL"),
                new SqlTypeInfo("TEXT"),
                new SqlTypeInfo("BLOB"),
            };

            return sqlTypeInfos;
        }
    }
}