namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System;
    using System.Text;
    using FizzCode.DbTools.DataDefinition;

    public class ColumnCreationHelper
    {
        public static string GetColumnCreation(SqlColumn column)
        {
            var sb = new StringBuilder();
            sb.Append("\t\t\ttable.");
            sb.Append(GetColumnCreationMethod(column));


            if (column.IsNullable)
                sb.Append(", true");

            // TODO nullable
            // TODO: PKK, Identity etc.

            sb.Append(");");

            return sb.ToString();
        }

        public static string GetColumnCreationMethod(SqlColumn column)
        {
            switch (column.Type)
            {
                case SqlType.Boolean:
                    return $"AddBoolean(\"{column.Name}\"";
                case SqlType.Byte:
                    return $"AddByte(\"{column.Name}\"";
                case SqlType.Int16:
                    return $"AddInt16(\"{column.Name}\"";
                case SqlType.Int32:
                    return $"AddInt32(\"{column.Name}\"";
                case SqlType.Int64:
                    return $"AddInt64(\"{column.Name}\"";

                case SqlType.NVarchar:
                    return $"AddNVarChar(\"{column.Name}\", {column.Length}";
                case SqlType.Varchar:
                    return $"AddVarChar(\"{column.Name}\", {column.Length}";
                case SqlType.NChar:
                    return $"AddNChar(\"{column.Name}\", {column.Length}";
                case SqlType.Char:
                    throw new NotImplementedException($"Implement SqlType: {Enum.GetName(typeof(SqlType), column.Type)}");

                case SqlType.Date:
                    return $"AddDate(\"{column.Name}\"";

                // TODO Datetime2 / offset?
                case SqlType.DateTime:
                    return $"AddDateTime(\"{column.Name}\"";

                case SqlType.Decimal:
                    return $"AddDecimal(\"{column.Name}\", " + (column.Length != null ? column.Length.ToString() : "null") + (column.Precision != null ? column.Precision.ToString() : "null");
                case SqlType.Double:
                    return $"AddDouble(\"{column.Name}\", " + (column.Length != null ? column.Length.ToString() : "null");

                case SqlType.Image:
                    return $"AddImage(\"{column.Name}\"";
                case SqlType.Guid:
                    return $"AddGuid(\"{column.Name}\"";
                /*case SqlType.Binary:
                    return "BINARY";
                case SqlType.VarBinary:
                    return "VARBINARY";
                case SqlType.Image :
                    return "IMAGE";
                case SqlType.NText:
                    return "NTEXT";*/
                default:
                    throw new NotImplementedException($"Unmapped SqlType: {Enum.GetName(typeof(SqlType), column.Type)}");
            }
        }
    }
}