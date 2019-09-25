namespace FizzCode.DbTools.DataDefinitionGenerator
{
    using System;
    using FizzCode.DbTools.DataDefinition;

    public class GenericSqlTypeMapper : ISqlTypeMapper
    {
        public virtual string GetType(SqlType type)
        {
            return type switch
            {
                SqlType.Boolean => "BIT",
                SqlType.Byte => "TINYINT",
                SqlType.Int16 => "SMALLINT",
                SqlType.Int32 => "INT",
                SqlType.Int64 => "BIGINT",

                SqlType.NVarchar => "NVARCHAR",
                SqlType.Varchar => "VARCHAR",
                SqlType.NChar => "NCHAR",
                SqlType.Char => "CHAR",

                SqlType.Date => "DATE",

                // TODO Datetime2 / offset?
                SqlType.DateTime => "DATETIME",
                SqlType.DateTimeOffset => "DATETIMEOFFSET",

                SqlType.Decimal => "DECIMAL",
                SqlType.Double => "FLOAT",
                SqlType.Money => "MONEY",

                SqlType.Xml => "XML",
                SqlType.Guid => "UNIQUEIDENTIFIER",
                SqlType.Binary => "BINARY",
                SqlType.VarBinary => "VARBINARY",
                SqlType.Image => "IMAGE",
                SqlType.NText => "NTEXT",
                SqlType.Single => "REAL",
                _ => throw new NotImplementedException($"Unmapped SqlType: {Enum.GetName(typeof(SqlType), type)}"),
            };
        }
    }
}