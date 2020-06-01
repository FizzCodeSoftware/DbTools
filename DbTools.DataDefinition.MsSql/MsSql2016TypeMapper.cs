namespace FizzCode.DbTools.DataDefinition.MsSql2016
{
    using System;
    using System.Globalization;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition.Generic1;

    public class MsSql2016TypeMapper : AbstractTypeMapper
    {
        public override SqlEngineVersion SqlVersion => MsSqlVersion.MsSql2016;

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
                DataDefinition.Generic1.SqlChar _ => genericType.Clone(MsSqlType2016.Char),
                DataDefinition.Generic1.SqlNChar _ => genericType.Clone(MsSqlType2016.NChar),
                DataDefinition.Generic1.SqlVarChar _ => genericType.Clone(MsSqlType2016.VarChar),
                DataDefinition.Generic1.SqlNVarChar _ => genericType.Clone(MsSqlType2016.NVarChar),
                SqlFloatSmall _ => genericType.Clone(MsSqlType2016.Float),
                SqlFloatLarge _ => genericType.Clone(MsSqlType2016.Real),
                DataDefinition.Generic1.SqlBit _ => genericType.Clone(MsSqlType2016.Bit),
                SqlByte _ => genericType.Clone(MsSqlType2016.TinyInt),
                SqlInt16 _ => genericType.Clone(MsSqlType2016.SmallInt),
                SqlInt32 _ => genericType.Clone(MsSqlType2016.Int),
                SqlInt64 _ => genericType.Clone(MsSqlType2016.BigInt),
                DataDefinition.Generic1.SqlDateTime _ => genericType.Clone(MsSqlType2016.DateTime),
                DataDefinition.Generic1.SqlDate _ => genericType.Clone(MsSqlType2016.Date),
                _ => throw new NotImplementedException($"Unmapped type {genericType.SqlTypeInfo}"),
            };
        }

        public override SqlType MapToGeneric1(SqlType sqlType)
        {
            return sqlType.SqlTypeInfo switch
            {
                SqlChar _ => sqlType.Clone(GenericSqlType1.Char),
                SqlNChar _ => sqlType.Clone(GenericSqlType1.NChar),
                SqlVarChar _ => sqlType.Clone(GenericSqlType1.VarChar),
                SqlNVarChar _ => sqlType.Clone(GenericSqlType1.NVarChar),
                SqlText _ => sqlType.Clone(GenericSqlType1.Text),
                SqlNText _ => sqlType.Clone(GenericSqlType1.Text),
                SqlFloat _ => sqlType.Clone(GenericSqlType1.FloatSmall),
                SqlReal _ => sqlType.Clone(GenericSqlType1.FloatLarge),
                SqlBit _ => sqlType.Clone(GenericSqlType1.Bit),
                SqlTinyInt _ => sqlType.Clone(GenericSqlType1.Byte),
                SqlSmallInt _ => sqlType.Clone(GenericSqlType1.Int16),
                SqlInt _ => sqlType.Clone(GenericSqlType1.Int32),
                SqlBigInt _ => sqlType.Clone(GenericSqlType1.Int64),
                SqlDateTime _ => sqlType.Clone(GenericSqlType1.DateTime),
                SqlDateTime2 _ => sqlType.Clone(GenericSqlType1.DateTime),
                SqlDate _ => sqlType.Clone(GenericSqlType1.Date),
                SqlSmallDateTime _ => sqlType.Clone(GenericSqlType1.Date),
                SqlDecimal _ => sqlType.Clone(GenericSqlType1.Number),
                SqlNumeric _ => sqlType.Clone(GenericSqlType1.Number),
                SqlMoney _ => sqlType.Clone(GenericSqlType1.Number),
                SqlXml _ => sqlType.Clone(GenericSqlType1.Text),
                SqlUniqueIdentifier _ => sqlType.Clone(GenericSqlType1.Text),
                SqlBinary _ => sqlType.Clone(GenericSqlType1.Text),
                SqlVarBinary _ => sqlType.Clone(GenericSqlType1.Text),
                SqlImage _ => sqlType.Clone(GenericSqlType1.Text),
                _ => throw new NotImplementedException($"Unmapped type {sqlType.SqlTypeInfo}"),
            };
        }
    }
}
