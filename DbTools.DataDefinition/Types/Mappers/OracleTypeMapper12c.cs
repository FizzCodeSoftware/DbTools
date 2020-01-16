namespace FizzCode.DbTools.DataDefinition
{
    using System;

    public class OracleTypeMapper12c : TypeMapper
    {
        public OracleTypeMapper12c()
        {
            SqlTypeInfos = GetTypeInfos();
        }

        public SqlType MapSqlType(string type, bool isNullable, int dataPrecision, int dataScale)
        {
            // TODO VARCHAR2(20 BYTE) VS VARCHAR2(20 CHAR)

            switch (type.ToUpper())
            {
                case "CHAR":
                case "NCHAR":
                case "VARCHAR":
                case "NVARCHAR":
                case "VARCHAR2":
                case "NVARCHAR2":
                    return MapSqlType(type, isNullable, dataPrecision);

                case "BLOB":
                case "CLOB":
                case "NCLOB":
                case "BFILE":
                case "LONG": // TODO handle deprecated
                case "LONG RAW": // TODO handle deprecated
                    return MapSqlType(type, isNullable, dataPrecision);

                case "NUMBER":
                    return MapSqlType(type, isNullable, dataPrecision, dataScale);

                case "BINARY_FLOAT":
                case "BINARY_DOUBLE":
                    return MapSqlType(type, isNullable);

                case "DATE":
                case "TIMESTAMP WITH TIME ZONE":
                case "TIMESTAMP WITH LOCAL TIME ZONE":
                    return MapSqlType(type, isNullable);

                case "XMLTYPE":
                case "URITYPE":
                    return MapSqlType(type, isNullable);

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
                    return MapAs("VARCHAR2", genericType);
                case "NVARCHAR":
                    return MapAs("NVARCHAR2", genericType);
                case "FLOAT_SMALL":
                    return MapAs("BINARY_FLOAT", genericType);
                case "FLOAT_LARGE":
                    return MapAs("BINARY_DOUBLE", genericType);
                case "BIT":
                case "BYTE":
                case "INT16":
                case "INT32":
                case "INT64":
                    return genericType.CopyTo(result);
                default:
                    throw new NotImplementedException($"Unmapped type {genericType.SqlTypeInfo.DbType}");
            }
        }

        protected override SqlTypeInfos GetTypeInfos()
        {
            var sqlTypeInfos = new SqlTypeInfos
            {
                new SqlTypeInfo("CHAR", true, false, false),
                new SqlTypeInfo("NCHAR", true, false, false),
                new SqlTypeInfo("VARCHAR", true, false, false),
                new SqlTypeInfo("NVARCHAR", true, false, false),
                new SqlTypeInfo("VARCHAR2", true, false, false),
                new SqlTypeInfo("NVARCHAR2", true, false, false),

                new SqlTypeInfo("BLOB", false, false, false),
                new SqlTypeInfo("CLOB", false, false, false),
                new SqlTypeInfo("NCLOB", false, false, false),
                new SqlTypeInfo("BFILE", false, false, false),
                new SqlTypeInfo("LONG", false, false, true),
                new SqlTypeInfo("LONG RAW", false, false, true),
                // BINARY, VARBINARY
                // INTERVAL 
                // TIME
                // TT*

                new SqlTypeInfo("NUMBER", true, false, true, false, true),

                new SqlTypeInfo("BINARY_FLOAT"),
                new SqlTypeInfo("BINARY_DOUBLE"),

                new SqlTypeInfo("DATE"),
                new SqlTypeInfo("TIMESTAMP WITH TIME ZONE"),
                new SqlTypeInfo("TIMESTAMP WITH LOCAL TIME ZONE"),

                new SqlTypeInfo("XMLType"),
                new SqlTypeInfo("UriType")
            };

            return sqlTypeInfos;
        }
    }
}