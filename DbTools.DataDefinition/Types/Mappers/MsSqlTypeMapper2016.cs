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
                    return base.MapSqlType(new MsSql2016.Char(), isNullable, characterMaximumLength);
                case "NCHAR":
                    return base.MapSqlType(new MsSql2016.NChar(), isNullable, characterMaximumLength);
                case "VARCHAR": // TODO max length allowed - what is in Row?
                    return base.MapSqlType(new MsSql2016.VarChar(), isNullable, characterMaximumLength);
                case "NVARCHAR": // TODO max length allowed - what is in Row?
                    return base.MapSqlType(new MsSql2016.NVarChar(), isNullable, characterMaximumLength);
                case "BIT":
                case "TINYINT":
                    return base.MapSqlType(new MsSql2016.TinyInt(), isNullable);
                case "SMALLINT":
                    return base.MapSqlType(new MsSql2016.SmallInt(), isNullable);
                case "INT":
                    return base.MapSqlType(new MsSql2016.Int(), isNullable);
                case "BIGINT":
                    return base.MapSqlType(new MsSql2016.BigInt(), isNullable);
                case "DECIMAL":
                    return base.MapSqlType(new MsSql2016.Decimal(), isNullable);
                case "NUMERIC":
                    return base.MapSqlType(new MsSql2016.Numeric(), isNullable);
                case "MONEY":
                    return base.MapSqlType(new MsSql2016.Money(), isNullable);
                case "SMALLMONEY":
                    return base.MapSqlType(new MsSql2016.SmallMoney(), isNullable);
                case "FLOAT":
                    return base.MapSqlType(new MsSql2016.Float(), isNullable);
                case "REAL":
                    return base.MapSqlType(new MsSql2016.Real(), isNullable);
                case "DATE":
                    return base.MapSqlType(new MsSql2016.Date(), isNullable);
                case "DATETIME":
                    return base.MapSqlType(new MsSql2016.DateTime(), isNullable);
                case "SMALLDATETIME":
                    return base.MapSqlType(new MsSql2016.SmallDateTime(), isNullable);
                case "TIME":
                    return base.MapSqlType(new MsSql2016.Time(), isNullable, datetimePrecision);
                case "DATETIME2":
                    return base.MapSqlType(new MsSql2016.DateTime2(), isNullable, datetimePrecision);
                case "DATETIMEOFFSET":
                    return base.MapSqlType(new MsSql2016.DateTimeOffset(), isNullable, datetimePrecision);
                case "BINARY":
                    return base.MapSqlType(new MsSql2016.Binary(), isNullable, characterMaximumLength); // TODO which length?
                case "VARBINARY": // TODO max length allowed - what is in Row?
                    return base.MapSqlType(new MsSql2016.VarBinary(), isNullable, characterMaximumLength); // TODO which length?
                case "IMAGE":
                    return base.MapSqlType(new MsSql2016.Image(), isNullable);
                case "XML":
                    return base.MapSqlType(new MsSql2016.Xml(), isNullable);
                case "UNIQUEIDENTIFIER":
                    return base.MapSqlType(new MsSql2016.UniqueIdentifier(), isNullable);
                default:
                    throw new NotImplementedException($"Unmapped SqlType: {type}.");
            }
        }

        public override SqlType MapFromGeneric1(SqlType genericType)
        {
            return genericType.SqlTypeInfo switch
            {
                Generic1.Char _ => genericType.Create(typeof(MsSql2016.Char)),
                Generic1.NChar _ => genericType.Create(typeof(MsSql2016.NChar)),
                Generic1.VarChar _ => genericType.Create(typeof(MsSql2016.VarChar)),
                Generic1.NVarChar _ => genericType.Create(typeof(MsSql2016.NVarChar)),
                Generic1.FloatSmall _ => genericType.Create(typeof(MsSql2016.Float)),
                Generic1.FloatLarge _ => genericType.Create(typeof(MsSql2016.Real)),
                Generic1.Bit _ => genericType.Create(typeof(MsSql2016.Bit)),
                Generic1.Byte _ => genericType.Create(typeof(MsSql2016.TinyInt)),
                Generic1.Int16 _ => genericType.Create(typeof(MsSql2016.SmallInt)),
                Generic1.Int32 _ => genericType.Create(typeof(MsSql2016.Int)),
                Generic1.Int64 _ => genericType.Create(typeof(MsSql2016.BigInt)),
                Generic1.DateTime _ => genericType.Create(typeof(MsSql2016.DateTime)),
                Generic1.Date _ => genericType.Create(typeof(MsSql2016.Date)),
                _ => throw new NotImplementedException($"Unmapped type {genericType.SqlTypeInfo}"),
            };
        }
    }
}