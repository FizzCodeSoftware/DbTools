namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System;
    using System.Globalization;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Oracle12c;

    public class OracleCsGenerator12c : GeneratorTypeSpecific
    {
        public OracleCsGenerator12c(Context context) : base(context)
        {
            Version = SqlVersions.Oracle12c;
        }

        protected override string GetColumnCreationMethod(SqlColumn column)
        {
            var type = column.Types[Version];
            return type.SqlTypeInfo switch
            {
                SqlChar _ => $"{nameof(Oracle12cHelper.AddChar)}(\"{column.Name}\", {type.Length?.ToString("D", CultureInfo.InvariantCulture)}",
                SqlNChar _ => $"{nameof(Oracle12cHelper.AddNChar)}(\"{column.Name}\", {type.Length?.ToString("D", CultureInfo.InvariantCulture)}",
                SqlVarChar _ => $"{nameof(Oracle12cHelper.AddVarChar)}(\"{column.Name}\", {type.Length?.ToString("D", CultureInfo.InvariantCulture)}",
                SqlVarChar2 _ => $"{nameof(Oracle12cHelper.AddVarChar2)}(\"{column.Name}\", {type.Length?.ToString("D", CultureInfo.InvariantCulture)}",
                SqlNVarChar2 _ => $"{nameof(Oracle12cHelper.AddNVarChar2)}(\"{column.Name}\", {type.Length?.ToString("D", CultureInfo.InvariantCulture)}",
                SqlBinaryFloat _ => $"{nameof(Oracle12cHelper.AddBinaryFloat)}(\"{column.Name}\"",
                SqlBinaryDouble _ => $"{nameof(Oracle12cHelper.AddBinaryDouble)}(\"{column.Name}\"",
                SqlBfile _ => $"{nameof(Oracle12cHelper.AddBfile)}(\"{column.Name}\"",
                SqlBlob _ => $"{nameof(Oracle12cHelper.AddBlob)}(\"{column.Name}\"",
                SqlClob _ => $"{nameof(Oracle12cHelper.AddClob)}(\"{column.Name}\"",
                SqlLong _ => $"{nameof(Oracle12cHelper.AddLong)}(\"{column.Name}\"",
                SqlLongRaw _ => $"{nameof(Oracle12cHelper.AddLongRaw)}(\"{column.Name}\"",
                SqlNumber _ => $"{nameof(Oracle12cHelper.AddLongRaw)}(\"{column.Name}\", {type.Length?.ToString("D", CultureInfo.InvariantCulture)}, {type.Scale?.ToString("D", CultureInfo.InvariantCulture)}",
                SqlDate _ => $"{nameof(Oracle12cHelper.AddDate)}(\"{column.Name}\"",
                SqlTimeStampWithTimeZone _ => $"{nameof(Oracle12cHelper.AddTimeStampWithTimeZone)}(\"{column.Name}\"",
                SqlTimeStampWithLocalTimeZone _ => $"{nameof(Oracle12cHelper.AddTimeStampWithLocalTimeZone)}(\"{column.Name}\"",
                _ => throw new NotImplementedException($"Unmapped type: {type.SqlTypeInfo}"),
            };
        }

        /*
                public static string GetColumnCreationMethod(SqlColumn column)
                {
                    return column.Type switch
                    {
                        SqlType.Boolean => $"AddBoolean(\"{column.Name}\"",
                        SqlType.Byte => $"AddByte(\"{column.Name}\"",
                        SqlType.Int16 => $"AddInt16(\"{column.Name}\"",
                        SqlType.Int32 => $"AddInt32(\"{column.Name}\"",
                        SqlType.Int64 => $"AddInt64(\"{column.Name}\"",

                        SqlType.NVarchar => $"AddNVarChar(\"{column.Name}\", {column.Length}",
                        SqlType.Varchar => $"AddVarChar(\"{column.Name}\", {column.Length}",
                        SqlType.NChar => $"AddNChar(\"{column.Name}\", {column.Length}",
                        SqlType.Char => $"AddChar(\"{column.Name}\", {column.Length}",
                        SqlType.Date => $"AddDate(\"{column.Name}\"",

                        // TODO Datetime2 / offset?
                        SqlType.DateTime => $"AddDateTime(\"{column.Name}\"",
                        SqlType.DateTimeOffset => $"AddDateTimeOffset(\"{column.Name}\", " + column.Precision,

                        SqlType.Decimal => $"AddDecimal(\"{column.Name}\", " + (column.Length != null ? column.Length.ToString() : "null") + "," + (column.Precision != null ? column.Precision.ToString() : "null"),
                        SqlType.Double => $"AddDouble(\"{column.Name}\", " + (column.Length != null ? column.Length.ToString() : "null"),

                        SqlType.Image => $"AddImage(\"{column.Name}\"",
                        SqlType.Guid => $"AddGuid(\"{column.Name}\"",
                        SqlType.Xml => $"AddXml(\"{column.Name}\"",
                        //case SqlType.Binary:
                        //    return "BINARY";
                        //case SqlType.VarBinary:
                        //    return "VARBINARY";
                        //case SqlType.Image :
                        //    return "IMAGE";
                        //case SqlType.NText:
                        //    return "NTEXT";
        _ => throw new NotImplementedException($"Unmapped SqlType: {Enum.GetName(typeof(SqlType), column.Type)}"),
            };
        }
*/
    }
}
