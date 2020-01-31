namespace FizzCode.DbTools.DataDefinition
{
    using System;

    public class OracleTypeMapper12c : TypeMapper
    {
        public SqlType MapSqlTypeFromReaderInfo(string type, bool isNullable, int dataPrecision, int dataScale)
        {
            // TODO VARCHAR2(20 BYTE) VS VARCHAR2(20 CHAR)

            switch (type.ToUpper())
            {
                case "CHAR":
                    return base.MapSqlType(new Oracle12c.Char(), isNullable, dataPrecision);
                case "NCHAR":
                    return base.MapSqlType(new Oracle12c.NChar(), isNullable, dataPrecision);
                case "VARCHAR":
                    return base.MapSqlType(new Oracle12c.VarChar(), isNullable, dataPrecision);
                case "VARCHAR2":
                    return base.MapSqlType(new Oracle12c.VarChar2(), isNullable, dataPrecision);
                case "NVARCHAR2":
                    return base.MapSqlType(new Oracle12c.NVarChar2(), isNullable, dataPrecision);

                case "BLOB":
                    return base.MapSqlType(new Oracle12c.Blob(), isNullable);
                case "CLOB":
                    return base.MapSqlType(new Oracle12c.Clob(), isNullable);
                case "NCLOB":
                    return base.MapSqlType(new Oracle12c.NClob(), isNullable);
                case "BFILE":
                    return base.MapSqlType(new Oracle12c.Bfile(), isNullable);
                case "LONG": // TODO handle deprecated
                    return base.MapSqlType(new Oracle12c.Long(), isNullable, dataPrecision);
                case "LONG RAW": // TODO handle deprecated
                    return base.MapSqlType(new Oracle12c.Bfile(), isNullable, dataPrecision);

                case "NUMBER":
                    return base.MapSqlType(new Oracle12c.Number(), isNullable, dataPrecision, dataScale);

                case "BINARY_FLOAT":
                    return base.MapSqlType(new Oracle12c.BinaryFloat(), isNullable);
                case "BINARY_DOUBLE":
                    return base.MapSqlType(new Oracle12c.BinaryDouble(), isNullable);

                case "DATE":
                    return base.MapSqlType(new Oracle12c.Date(), isNullable);
                case "TIMESTAMP WITH TIME ZONE":
                    return base.MapSqlType(new Oracle12c.TimeStampWithTimeZone(), isNullable);
                case "TIMESTAMP WITH LOCAL TIME ZONE":
                    return base.MapSqlType(new Oracle12c.TimeStampWithLocalTimeZone(), isNullable);

                case "XMLTYPE":
                    return base.MapSqlType(new Oracle12c.XmlType(), isNullable);
                case "URITYPE":
                    return base.MapSqlType(new Oracle12c.UriType(), isNullable);

                default:
                    throw new NotImplementedException($"Unmapped SqlType: {type}.");
            }
        }

        public override SqlType MapFromGeneric1(SqlType genericType)
        {
            return genericType.SqlTypeInfo switch
            {
                Generic1.Char _ => genericType.Create(typeof(Oracle12c.Char)),
                Generic1.NChar _ => genericType.Create(typeof(Oracle12c.NChar)),
                Generic1.VarChar _ => genericType.Create(typeof(Oracle12c.VarChar)),
                Generic1.NVarChar _ => genericType.Create(typeof(Oracle12c.NVarChar2)),
                Generic1.FloatSmall _ => genericType.Create(typeof(Oracle12c.BinaryFloat)),
                Generic1.FloatLarge _ => genericType.Create(typeof(Oracle12c.BinaryDouble)),
                Generic1.Bit _ => genericType.Create(typeof(Oracle12c.Number)),
                Generic1.Byte _ => genericType.Create(typeof(Oracle12c.Number)),
                Generic1.Int16 _ => genericType.Create(typeof(Oracle12c.Number)),
                Generic1.Int32 _ => genericType.Create(typeof(Oracle12c.Number)),
                Generic1.Int64 _ => genericType.Create(typeof(Oracle12c.Number)),
                Generic1.DateTime _ => genericType.Create(typeof(Oracle12c.Date)),
                Generic1.Date _ => genericType.Create(typeof(Oracle12c.Date)),
                _ => throw new NotImplementedException($"Unmapped type {genericType.SqlTypeInfo}"),
            };
        }
    }
}