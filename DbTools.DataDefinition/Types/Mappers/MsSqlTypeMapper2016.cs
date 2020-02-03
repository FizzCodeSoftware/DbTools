namespace FizzCode.DbTools.DataDefinition
{
    using System;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition.Generic1;

    public class GenericTypeMapper1 : TypeMapper
    {
        public override SqlVersion SqlVersion => SqlEngines.Generic1;

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
        public override SqlVersion SqlVersion => SqlEngines.MsSql2016;

        public SqlType MapSqlTypeFromReaderInfo(string type, bool isNullable, int numericPrecision, int numericSale, int characterMaximumLength, int datetimePrecision)
        {
            switch (type.ToUpper())
            {
                case "CHAR":
                    return base.MapSqlType(MsSqlType2016.Char, isNullable, characterMaximumLength);
                case "NCHAR":
                    return base.MapSqlType(MsSqlType2016.NChar, isNullable, characterMaximumLength);
                case "VARCHAR": // TODO max length allowed - what is in Row?
                    return base.MapSqlType(MsSqlType2016.VarChar, isNullable, characterMaximumLength);
                case "NVARCHAR": // TODO max length allowed - what is in Row?
                    return base.MapSqlType(MsSqlType2016.NVarChar, isNullable, characterMaximumLength);
                case "BIT":
                    return base.MapSqlType(MsSqlType2016.Bit, isNullable);
                case "TINYINT":
                    return base.MapSqlType(MsSqlType2016.TinyInt, isNullable);
                case "SMALLINT":
                    return base.MapSqlType(MsSqlType2016.SmallInt, isNullable);
                case "INT":
                    return base.MapSqlType(MsSqlType2016.Int, isNullable);
                case "BIGINT":
                    return base.MapSqlType(MsSqlType2016.BigInt, isNullable);
                case "DECIMAL":
                    return base.MapSqlType(MsSqlType2016.Decimal, isNullable, numericPrecision, numericSale);
                case "NUMERIC":
                    return base.MapSqlType(MsSqlType2016.Numeric, isNullable, numericPrecision, numericSale);
                case "MONEY":
                    return base.MapSqlType(MsSqlType2016.Money, isNullable);
                case "SMALLMONEY":
                    return base.MapSqlType(MsSqlType2016.SmallMoney, isNullable);
                case "FLOAT":
                    return base.MapSqlType(MsSqlType2016.Float, isNullable);
                case "REAL":
                    return base.MapSqlType(MsSqlType2016.Real, isNullable);
                case "DATE":
                    return base.MapSqlType(MsSqlType2016.Date, isNullable);
                case "DATETIME":
                    return base.MapSqlType(MsSqlType2016.DateTime, isNullable);
                case "SMALLDATETIME":
                    return base.MapSqlType(MsSqlType2016.SmallDateTime, isNullable);
                case "TIME":
                    return base.MapSqlType(MsSqlType2016.Time, isNullable, datetimePrecision);
                case "DATETIME2":
                    return base.MapSqlType(MsSqlType2016.DateTime2, isNullable, datetimePrecision);
                case "DATETIMEOFFSET":
                    return base.MapSqlType(MsSqlType2016.DateTimeOffset, isNullable, datetimePrecision);
                case "BINARY":
                    return base.MapSqlType(MsSqlType2016.Binary, isNullable, characterMaximumLength); // TODO which length?
                case "VARBINARY": // TODO max length allowed - what is in Row?
                    return base.MapSqlType(MsSqlType2016.VarBinary, isNullable, characterMaximumLength); // TODO which length?
                case "IMAGE":
                    return base.MapSqlType(MsSqlType2016.Image, isNullable);
                case "XML":
                    return base.MapSqlType(MsSqlType2016.Xml, isNullable);
                case "UNIQUEIDENTIFIER":
                    return base.MapSqlType(MsSqlType2016.UniqueIdentifier, isNullable);
                case "TEXT":
                    return base.MapSqlType(MsSqlType2016.Text, isNullable);
                case "NTEXT":
                    return base.MapSqlType(MsSqlType2016.NText, isNullable);
                default:
                    throw new NotImplementedException($"Unmapped SqlType: {type}.");
            }
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
