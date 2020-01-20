namespace FizzCode.DbTools.DataDefinition
{
    using System;

    public class GenericTypeMapper1 : TypeMapper
    {
        public GenericTypeMapper1()
        {
            SqlTypeInfos = GetTypeInfos();
        }

        public override SqlType MapFromGeneric1(SqlType genericType)
        {
            throw new NotImplementedException();
        }

        protected override SqlTypeInfos GetTypeInfos()
        {
            var sqlTypeInfos = new SqlTypeInfos
            {
                new SqlTypeInfo("CHAR", true, false, false),
                new SqlTypeInfo("NCHAR", true, false, false),
                new SqlTypeInfo("VARCHAR", true, false, false),
                new SqlTypeInfo("NVARCHAR", true, false, false),

                new SqlTypeInfo("DEFAULTPK"),

                new SqlTypeInfo("FLOAT_SMALL"),
                new SqlTypeInfo("FLOAT_LARGE"),

                new SqlTypeInfo("BIT"),
                new SqlTypeInfo("BYTE"),
                new SqlTypeInfo("INT16", true, true, false),
                new SqlTypeInfo("INT32", true, true, false),
                new SqlTypeInfo("INT64", true, true, false),

                new SqlTypeInfo("NUMBER", true, true, false),

                new SqlTypeInfo("DATE"),
                new SqlTypeInfo("TIME"),
                new SqlTypeInfo("DATETIME"),

                // TODO Blob and like
            };

            return sqlTypeInfos;
        }
    }

    public class MsSqlTypeMapper2016 : TypeMapper
    {
        public MsSqlTypeMapper2016()
        {
            SqlTypeInfos = GetTypeInfos();
        }

        protected override SqlTypeInfos GetTypeInfos()
        {
            var sqlTypeInfos = new SqlTypeInfos
            {
                new SqlTypeInfo("CHAR", true, false, false),
                new SqlTypeInfo("NCHAR", true, false, false),
                new SqlTypeInfo("VARCHAR", true),
                new SqlTypeInfo("NVARCHAR", true),
                new SqlTypeInfo("NTEXT"),

                new SqlTypeInfo("BIT"),
                new SqlTypeInfo("TINYINT"),
                new SqlTypeInfo("SMALLINT"),
                new SqlTypeInfo("INT"),
                new SqlTypeInfo("BIGINT"),

                new SqlTypeInfo("DECIMAL", true, true, false),
                new SqlTypeInfo("NUMERIC", true, true, false),
                new SqlTypeInfo("MONEY"),
                new SqlTypeInfo("SMALLMONEY"),

                new SqlTypeInfo("FLOAT"),
                new SqlTypeInfo("REAL"),

                new SqlTypeInfo("DATE"),
                new SqlTypeInfo("TIME", true, false, false),
                new SqlTypeInfo("DATETIME"),
                new SqlTypeInfo("DATETIME2", true, false, false),
                new SqlTypeInfo("DATETIMEOFFSET", true, false, false),
                new SqlTypeInfo("SMALLDATETIME"),

                new SqlTypeInfo("BINARY", true, false, false),
                new SqlTypeInfo("VARBINARY", true),
                new SqlTypeInfo("IMAGE"),

                new SqlTypeInfo("XML"),
            };

            return sqlTypeInfos;
        }

        public SqlType MapSqlTypeFromReaderInfo(string type, bool isNullable, int numericPrecision, int numericSale, int characterMaximumLength, int datetimePrecision)
        {
            switch (type.ToUpper())
            {
                case "CHAR":
                case "NCHAR":
                case "VARCHAR": // TODO max length allowed - what is in Row?
                case "NVARCHAR": // TODO max length allowed - what is in Row?
                    return base.MapSqlType(type, isNullable, characterMaximumLength);
                case "BIT":
                case "TINYINT":
                case "SMALLINT":
                case "INT":
                case "BIGINT":
                    return base.MapSqlType(type, isNullable);
                case "DECIMAL":
                case "NUMERIC":
                    return base.MapSqlType(type, isNullable, numericPrecision, numericSale);
                case "MONEY":
                case "SMALLMONEY":
                    return base.MapSqlType(type, isNullable);
                case "FLOAT":
                case "REAL":
                    return base.MapSqlType(type, isNullable);
                case "DATE":
                case "DATETIME":
                case "SMALLDATETIME":
                    return base.MapSqlType(type, isNullable);
                case "TIME":
                case "DATETIME2":
                case "DATETIMEOFFSET":
                    return base.MapSqlType(type, isNullable, datetimePrecision);
                case "BINARY":
                    return base.MapSqlType(type, isNullable, characterMaximumLength); // TODO which length?
                case "VARBINARY": // TODO max length allowed - what is in Row?
                    return base.MapSqlType(type, isNullable, characterMaximumLength); // TODO which length?
                case "IMAGE":
                case "XML":
                    return base.MapSqlType(type, isNullable);
                default:
                    throw new NotImplementedException($"Unmapped SqlType: {type}.");
            }
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
                    return genericType.CopyTo(result);
                case "FLOAT_SMALL":
                    return MapAs("FLOAT", genericType);
                case "FLOAT_LARGE":
                    return MapAs("REAL", genericType);
                case "BIT":
                    return genericType.CopyTo(result);
                case "BYTE":
                    return MapAs("TINYINT", genericType);
                case "INT16":
                    return MapAs("SMALLINT", genericType);
                case "INT32":
                    return MapAs("INT", genericType);
                case "INT64":
                    return MapAs("BIGINT", genericType);
                default:
                    throw new NotImplementedException($"Unmapped type {genericType.SqlTypeInfo.DbType}");
            }
        }
    }
}