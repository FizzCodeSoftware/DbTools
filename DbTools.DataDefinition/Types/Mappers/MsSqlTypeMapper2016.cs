namespace FizzCode.DbTools.DataDefinition
{
    using System;
    using System.Globalization;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition.Generic1;

    public class GenericTypeMapper1 : TypeMapper
    {
        public override SqlVersion SqlVersion => SqlVersions.Generic1;

        public override SqlType MapFromGeneric1(SqlType genericType)
        {
            throw new NotImplementedException();
        }

        public override SqlType MapToGeneric1(SqlType sqlType)
        {
            throw new NotImplementedException();
        }
    }

    public class MsSqlTypeMapper2016 : TypeMapper
    {
        public override SqlVersion SqlVersion => SqlVersions.MsSql2016;

        public SqlType MapSqlTypeFromReaderInfo(string type, bool isNullable, int numericPrecision, int numericSale, int characterMaximumLength, int datetimePrecision)
        {
            return (type.ToUpper(CultureInfo.InvariantCulture)) switch
            {
                "CHAR" => base.MapSqlType(MsSqlType2016.Char, isNullable, characterMaximumLength),
                "NCHAR" => base.MapSqlType(MsSqlType2016.NChar, isNullable, characterMaximumLength),
                // TODO max length allowed - what is in Row?
                "VARCHAR" => base.MapSqlType(MsSqlType2016.VarChar, isNullable, characterMaximumLength),
                // TODO max length allowed - what is in Row?
                "NVARCHAR" => base.MapSqlType(MsSqlType2016.NVarChar, isNullable, characterMaximumLength),
                "BIT" => base.MapSqlType(MsSqlType2016.Bit, isNullable),
                "TINYINT" => base.MapSqlType(MsSqlType2016.TinyInt, isNullable),
                "SMALLINT" => base.MapSqlType(MsSqlType2016.SmallInt, isNullable),
                "INT" => base.MapSqlType(MsSqlType2016.Int, isNullable),
                "BIGINT" => base.MapSqlType(MsSqlType2016.BigInt, isNullable),
                "DECIMAL" => base.MapSqlType(MsSqlType2016.Decimal, isNullable, numericPrecision, numericSale),
                "NUMERIC" => base.MapSqlType(MsSqlType2016.Numeric, isNullable, numericPrecision, numericSale),
                "MONEY" => base.MapSqlType(MsSqlType2016.Money, isNullable),
                "SMALLMONEY" => base.MapSqlType(MsSqlType2016.SmallMoney, isNullable),
                "FLOAT" => base.MapSqlType(MsSqlType2016.Float, isNullable),
                "REAL" => base.MapSqlType(MsSqlType2016.Real, isNullable),
                "DATE" => base.MapSqlType(MsSqlType2016.Date, isNullable),
                "DATETIME" => base.MapSqlType(MsSqlType2016.DateTime, isNullable),
                "SMALLDATETIME" => base.MapSqlType(MsSqlType2016.SmallDateTime, isNullable),
                "TIME" => base.MapSqlType(MsSqlType2016.Time, isNullable, datetimePrecision),
                "DATETIME2" => base.MapSqlType(MsSqlType2016.DateTime2, isNullable, datetimePrecision),
                "DATETIMEOFFSET" => base.MapSqlType(MsSqlType2016.DateTimeOffset, isNullable, datetimePrecision),
                "BINARY" => base.MapSqlType(MsSqlType2016.Binary, isNullable, characterMaximumLength),// TODO which length?
                                                                                                      // TODO max length allowed - what is in Row?
                "VARBINARY" => base.MapSqlType(MsSqlType2016.VarBinary, isNullable, characterMaximumLength),// TODO which length?
                "IMAGE" => base.MapSqlType(MsSqlType2016.Image, isNullable),
                "XML" => base.MapSqlType(MsSqlType2016.Xml, isNullable),
                "UNIQUEIDENTIFIER" => base.MapSqlType(MsSqlType2016.UniqueIdentifier, isNullable),
                "TEXT" => base.MapSqlType(MsSqlType2016.Text, isNullable),
                "NTEXT" => base.MapSqlType(MsSqlType2016.NText, isNullable),
                _ => throw new NotImplementedException($"Unmapped SqlType: {type}."),
            };
        }

        public override SqlType MapFromGeneric1(SqlType genericType)
        {
            return genericType.SqlTypeInfo switch
            {
                Generic1.SqlChar _ => genericType.Create(MsSqlType2016.Char),
                Generic1.SqlNChar _ => genericType.Create(MsSqlType2016.NChar),
                Generic1.SqlVarChar _ => genericType.Create(MsSqlType2016.VarChar),
                Generic1.SqlNVarChar _ => genericType.Create(MsSqlType2016.NVarChar),
                Generic1.SqlFloatSmall _ => genericType.Create(MsSqlType2016.Float),
                Generic1.SqlFloatLarge _ => genericType.Create(MsSqlType2016.Real),
                Generic1.SqlBit _ => genericType.Create(MsSqlType2016.Bit),
                Generic1.SqlByte _ => genericType.Create(MsSqlType2016.TinyInt),
                Generic1.SqlInt16 _ => genericType.Create(MsSqlType2016.SmallInt),
                Generic1.SqlInt32 _ => genericType.Create(MsSqlType2016.Int),
                Generic1.SqlInt64 _ => genericType.Create(MsSqlType2016.BigInt),
                Generic1.SqlDateTime _ => genericType.Create(MsSqlType2016.DateTime),
                Generic1.SqlDate _ => genericType.Create(MsSqlType2016.Date),
                _ => throw new NotImplementedException($"Unmapped type {genericType.SqlTypeInfo}"),
            };
        }

        public override SqlType MapToGeneric1(SqlType sqlType)
        {
            return sqlType.SqlTypeInfo switch
            {
                MsSql2016.SqlChar  _ => sqlType.Create(GenericSqlType1.Char),
                MsSql2016.SqlNChar _ => sqlType.Create(GenericSqlType1.NChar),
                MsSql2016.SqlVarChar _ => sqlType.Create(GenericSqlType1.VarChar),
                MsSql2016.SqlNVarChar _ => sqlType.Create(GenericSqlType1.NVarChar),
                MsSql2016.SqlFloat  _ => sqlType.Create(GenericSqlType1.FloatSmall),
                MsSql2016.SqlReal _ => sqlType.Create(GenericSqlType1.FloatLarge),
                MsSql2016.SqlBit _ => sqlType.Create(GenericSqlType1.Bit),
                MsSql2016.SqlTinyInt _ => sqlType.Create(GenericSqlType1.Byte),
                MsSql2016.SqlSmallInt _ => sqlType.Create(GenericSqlType1.Int16),
                MsSql2016.SqlInt _ => sqlType.Create(GenericSqlType1.Int32),
                MsSql2016.SqlBigInt _ => sqlType.Create(GenericSqlType1.Int64),
                MsSql2016.SqlDateTime _ => sqlType.Create(GenericSqlType1.DateTime),
                MsSql2016.SqlDateTime2 _ => sqlType.Create(GenericSqlType1.DateTime),
                MsSql2016.SqlDate _ => sqlType.Create(GenericSqlType1.Date),
                MsSql2016.SqlSmallDateTime _ => sqlType.Create(GenericSqlType1.Date),
                MsSql2016.SqlDecimal _ => sqlType.Create(GenericSqlType1.Number),
                MsSql2016.SqlNumeric _ => sqlType.Create(GenericSqlType1.Number),
                MsSql2016.SqlMoney _ => sqlType.Create(GenericSqlType1.Number),
                _ => throw new NotImplementedException($"Unmapped type {sqlType.SqlTypeInfo}"),
            };
        }
    }
}
