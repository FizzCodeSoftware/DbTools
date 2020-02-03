namespace FizzCode.DbTools.DataDefinition
{
    using System;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition.Generic1;

    public class OracleTypeMapper12c : TypeMapper
    {
        public override SqlVersion SqlVersion => SqlEngines.Oracle12c;

        public SqlType MapSqlTypeFromReaderInfo(string type, bool isNullable, int dataPrecision, int dataScale)
        {
            // TODO VARCHAR2(20 BYTE) VS VARCHAR2(20 CHAR)

            switch (type.ToUpper())
            {
                case "CHAR":
                    return base.MapSqlType(OracleType12c.Char, isNullable, dataPrecision);
                case "NCHAR":
                    return base.MapSqlType(OracleType12c.NChar, isNullable, dataPrecision);
                case "VARCHAR":
                    return base.MapSqlType(OracleType12c.VarChar, isNullable, dataPrecision);
                case "VARCHAR2":
                    return base.MapSqlType(OracleType12c.VarChar2, isNullable, dataPrecision);
                case "NVARCHAR2":
                    return base.MapSqlType(OracleType12c.NVarChar2, isNullable, dataPrecision);

                case "BLOB":
                    return base.MapSqlType(OracleType12c.Blob, isNullable);
                case "CLOB":
                    return base.MapSqlType(OracleType12c.Clob, isNullable);
                case "NCLOB":
                    return base.MapSqlType(OracleType12c.NClob, isNullable);
                case "BFILE":
                    return base.MapSqlType(OracleType12c.BFile, isNullable);
                case "LONG": // TODO handle deprecated
                    return base.MapSqlType(OracleType12c.Long, isNullable, dataPrecision);
                case "LONG RAW": // TODO handle deprecated
                    return base.MapSqlType(OracleType12c.LongRaw, isNullable, dataPrecision);

                case "NUMBER":
                    return base.MapSqlType(OracleType12c.Number, isNullable, dataPrecision, dataScale);

                case "BINARY_FLOAT":
                    return base.MapSqlType(OracleType12c.BinaryFloat, isNullable);
                case "BINARY_DOUBLE":
                    return base.MapSqlType(OracleType12c.BinaryDouble, isNullable);

                case "DATE":
                    return base.MapSqlType(OracleType12c.Date, isNullable);
                case "TIMESTAMP WITH TIME ZONE":
                    return base.MapSqlType(OracleType12c.TimeStampWithTimeZone, isNullable);
                case "TIMESTAMP WITH LOCAL TIME ZONE":
                    return base.MapSqlType(OracleType12c.TimeStampWithLocalTimeZone, isNullable);

                case "XMLTYPE":
                    return base.MapSqlType(OracleType12c.XmlType, isNullable);
                case "URITYPE":
                    return base.MapSqlType(OracleType12c.UriType, isNullable);

                default:
                    throw new NotImplementedException($"Unmapped SqlType: {type}.");
            }
        }

        public override SqlType MapFromGeneric1(SqlType genericType)
        {
            return genericType.SqlTypeInfo switch
            {
                Generic1.SqlChar _ => genericType.Create(OracleType12c.Char),
                Generic1.SqlNChar _ => genericType.Create(OracleType12c.NChar),
                Generic1.SqlVarChar _ => genericType.Create(OracleType12c.VarChar),
                Generic1.SqlNVarChar _ => genericType.Create(OracleType12c.NVarChar2),
                Generic1.SqlFloatSmall _ => genericType.Create(OracleType12c.BinaryFloat),
                Generic1.SqlFloatLarge _ => genericType.Create(OracleType12c.BinaryDouble),
                Generic1.SqlBit _ => genericType.Create(OracleType12c.Number),
                Generic1.SqlByte _ => genericType.Create(OracleType12c.Number),
                Generic1.SqlInt16 _ => genericType.Create(OracleType12c.Number),
                Generic1.SqlInt32 _ => genericType.Create(OracleType12c.Number),
                Generic1.SqlInt64 _ => genericType.Create(OracleType12c.Number),
                Generic1.SqlDateTime _ => genericType.Create(OracleType12c.Date),
                Generic1.SqlDate _ => genericType.Create(OracleType12c.Date),
                _ => throw new NotImplementedException($"Unmapped type {genericType.SqlTypeInfo}"),
            };
        }

        public override SqlType MapToGeneric1(SqlType sqlType)
        {
            return sqlType.SqlTypeInfo switch
            {
                Oracle12c.SqlChar _ => sqlType.Create(GenericSqlType1.Char),
                Oracle12c.SqlNChar _ => sqlType.Create(GenericSqlType1.NChar),
                Oracle12c.SqlVarChar _ => sqlType.Create(GenericSqlType1.VarChar),
                Oracle12c.SqlNVarChar2 _ => sqlType.Create(GenericSqlType1.NVarChar),
                Oracle12c.SqlBinaryFloat _ => sqlType.Create(GenericSqlType1.FloatSmall),
                Oracle12c.SqlBinaryDouble _ => sqlType.Create(GenericSqlType1.FloatLarge),
                // TODO iterations of number?
                // TODO store original?
                Oracle12c.SqlNumber _ => sqlType.Create(GenericSqlType1.Number),
                Oracle12c.SqlDate _ => sqlType.Create(GenericSqlType1.Date),
                _ => throw new NotImplementedException($"Unmapped type {sqlType.SqlTypeInfo}"),
            };
        }
    }
}