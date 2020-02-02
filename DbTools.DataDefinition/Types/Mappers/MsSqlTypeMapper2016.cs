namespace FizzCode.DbTools.DataDefinition
{
    using System;

    public class GenericTypeMapper1 : TypeMapper
    {
        public override SqlType MapFromGeneric1(SqlType genericType)
        {
            throw new NotImplementedException();
        }
    }

    public class MsSqlTypeMapper2016 : TypeMapper
    {
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
    }
}