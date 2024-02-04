using System;
using System.Globalization;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition.Base.Interfaces;
using FizzCode.DbTools.DataDefinition.Generic1;

namespace FizzCode.DbTools.DataDefinition.Oracle12c;
public class Oracle12cTypeMapper : AbstractTypeMapper
{
    public override SqlEngineVersion SqlVersion => OracleVersion.Oracle12c;

    public SqlType MapSqlTypeFromReaderInfo(string type, bool isNullable, int? charLength, int? dataPrecision, int? dataScale)
    {
        // TODO VARCHAR2(20 BYTE) VS VARCHAR2(20 CHAR)

        return type.ToUpper(CultureInfo.InvariantCulture) switch
        {
            "CHAR" => base.MapSqlType(OracleType12c.Char, isNullable, charLength),
            "NCHAR" => base.MapSqlType(OracleType12c.NChar, isNullable, charLength),
            "VARCHAR" => base.MapSqlType(OracleType12c.VarChar, isNullable, charLength),
            "VARCHAR2" => base.MapSqlType(OracleType12c.VarChar2, isNullable, charLength),
            "NVARCHAR2" => base.MapSqlType(OracleType12c.NVarChar2, isNullable, charLength),
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

    public override ISqlType MapFromGeneric1(ISqlType genericType)
    {
        return genericType.SqlTypeInfo switch
        {
            Generic1.SqlChar _ => genericType.Clone(OracleType12c.Char),
            Generic1.SqlNChar _ => genericType.Clone(OracleType12c.NChar),
            Generic1.SqlVarChar _ => genericType.Clone(OracleType12c.VarChar),
            SqlNVarChar _ => genericType.Clone(OracleType12c.NVarChar2),
            SqlFloatSmall _ => genericType.Clone(OracleType12c.BinaryFloat),
            SqlFloatLarge _ => genericType.Clone(OracleType12c.BinaryDouble),
            SqlBit _ => genericType.Clone(OracleType12c.Number),
            SqlByte _ => genericType.Clone(OracleType12c.Number),
            SqlInt16 _ => genericType.Clone(OracleType12c.Number, 4, 0),
            SqlInt32 _ => genericType.Clone(OracleType12c.Number, 9, 0),
            SqlInt64 _ => genericType.Clone(OracleType12c.Number, 19, 0),
            SqlDateTime _ => genericType.Clone(OracleType12c.Date),
            Generic1.SqlDate _ => genericType.Clone(OracleType12c.Date),
            _ => throw new NotImplementedException($"Unmapped type {genericType.SqlTypeInfo}"),
        };
    }

    public override ISqlType MapToGeneric1(ISqlType sqlType)
    {
        return sqlType.SqlTypeInfo switch
        {
            SqlChar _ => sqlType.Clone(GenericSqlType1.Char),
            SqlNChar _ => sqlType.Clone(GenericSqlType1.NChar),
            SqlVarChar _ => sqlType.Clone(GenericSqlType1.VarChar),
            SqlNVarChar2 _ => sqlType.Clone(GenericSqlType1.NVarChar),
            SqlBinaryFloat _ => sqlType.Clone(GenericSqlType1.FloatSmall),
            SqlBinaryDouble _ => sqlType.Clone(GenericSqlType1.FloatLarge),
            // TODO iterations of number?
            // TODO store original?
            SqlNumber _ => sqlType.Clone(GenericSqlType1.Number),
            SqlDate _ => sqlType.Clone(GenericSqlType1.Date),
            _ => throw new NotImplementedException($"Unmapped type {sqlType.SqlTypeInfo}"),
        };
    }
}