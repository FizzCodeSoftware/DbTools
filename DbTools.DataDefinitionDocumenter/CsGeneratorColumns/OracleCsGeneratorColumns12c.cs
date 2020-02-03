namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;

    public class OracleCsGeneratorColumns12c : GeneratorColumns
    {
        public OracleCsGeneratorColumns12c(Context context) : base(context)
        {
            Version = SqlEngines.Oracle12c;
        }

        protected override string GetColumnCreationMethod(SqlColumn column)
        {
            throw new NotImplementedException();
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