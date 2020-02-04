namespace FizzCode.DbTools.DataDefinition
{
    using System;
    using System.Globalization;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition.Generic1;

    public class OracleTypeMapper12c : TypeMapper
    {
        public override SqlVersion SqlVersion => SqlVersions.Oracle12c;

        public SqlType MapSqlTypeFromReaderInfo(string type, bool isNullable, int dataPrecision, int dataScale)
        {
            // TODO VARCHAR2(20 BYTE) VS VARCHAR2(20 CHAR)

            return (type.ToUpper(CultureInfo.InvariantCulture)) switch
            {
                "CHAR" => base.MapSqlType(OracleType12c.Char, isNullable, dataPrecision),
                "NCHAR" => base.MapSqlType(OracleType12c.NChar, isNullable, dataPrecision),
                "VARCHAR" => base.MapSqlType(OracleType12c.VarChar, isNullable, dataPrecision),
                "VARCHAR2" => base.MapSqlType(OracleType12c.VarChar2, isNullable, dataPrecision),
                "NVARCHAR2" => base.MapSqlType(OracleType12c.NVarChar2, isNullable, dataPrecision),
                "BLOB" => base.MapSqlType(OracleType12c.Blob, isNullable),
                "CLOB" => base.MapSqlType(OracleType12c.Clob, isNullable),
                "NCLOB" => base.MapSqlType(OracleType12c.NClob, isNullable),
                "BFILE" => base.MapSqlType(OracleType12c.BFile, isNullable),
                // TODO handle deprecated
                "LONG" => base.MapSqlType(OracleType12c.Long, isNullable, dataPrecision),
                // TODO handle deprecated
                "LONG RAW" => base.MapSqlType(OracleType12c.LongRaw, isNullable, dataPrecision),
                "NUMBER" => base.MapSqlType(OracleType12c.Number, isNullable, dataPrecision, dataScale),
                "BINARY_FLOAT" => base.MapSqlType(OracleType12c.BinaryFloat, isNullable),
                "BINARY_DOUBLE" => base.MapSqlType(OracleType12c.BinaryDouble, isNullable),
                "DATE" => base.MapSqlType(OracleType12c.Date, isNullable),
                "TIMESTAMP WITH TIME ZONE" => base.MapSqlType(OracleType12c.TimeStampWithTimeZone, isNullable),
                "TIMESTAMP WITH LOCAL TIME ZONE" => base.MapSqlType(OracleType12c.TimeStampWithLocalTimeZone, isNullable),
                "XMLTYPE" => base.MapSqlType(OracleType12c.XmlType, isNullable),
                "URITYPE" => base.MapSqlType(OracleType12c.UriType, isNullable),
                _ => throw new NotImplementedException($"Unmapped SqlType: {type}."),
            };
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