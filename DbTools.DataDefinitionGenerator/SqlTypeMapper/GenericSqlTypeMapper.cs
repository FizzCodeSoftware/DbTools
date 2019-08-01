namespace FizzCode.DbTools.DataDefinitionGenerator
{
    using System;
    using FizzCode.DbTools.DataDefinition;

    public class GenericSqlTypeMapper : ISqlTypeMapper
    {
        public virtual string GetType(SqlType type)
        {
            switch (type)
            {
                case SqlType.Boolean:
                case SqlType.Byte:
                    return "TINYINT";
                case SqlType.Int16:
                    return "SMALLINT";
                case SqlType.Int32:
                    return "INT";
                case SqlType.Int64:
                    return "BIGINT";

                case SqlType.NVarchar:
                    return "NVARCHAR";
                case SqlType.Varchar:
                    return "VARCHAR";
                case SqlType.NChar:
                    return "NCHAR";
                case SqlType.Char:
                    return "CHAR";

                case SqlType.Date:
                    return "DATE";

                // TODO Datetime2 / offset?
                case SqlType.DateTime:
                    return "DATETIME";

                case SqlType.Decimal:
                    return "DECIMAL";
                case SqlType.Double:
                    return "FLOAT";

                case SqlType.Xml:
                    return "XML";
                case SqlType.Guid:
                    return "UNIQUEIDENTIFIER";
                case SqlType.Binary:
                    return "BINARY";
                case SqlType.VarBinary:
                    return "VARBINARY";
                case SqlType.Image :
                    return "IMAGE";
                case SqlType.NText:
                    return "NTEXT";
                default:
                    throw new NotImplementedException($"Unmapped SqlType: {Enum.GetName(typeof(SqlType), type)}");
            }
        }
    }
}