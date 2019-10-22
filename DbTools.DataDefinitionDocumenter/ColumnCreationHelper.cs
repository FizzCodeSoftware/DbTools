namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System;
    using System.Linq;
    using System.Text;
    using FizzCode.DbTools.DataDefinition;

    public static class ColumnCreationHelper
    {
        public static string GetColumnCreation(SqlColumn column)
        {
            var sb = new StringBuilder();

            sb.Append(3, "table.")
                .Append(GetColumnCreationMethod(column));

            // TODO Type as ISqlTypeMapper

            if (column.IsNullable)
                sb.Append(", true");

            sb.Append(")");

            if (column.Table.Properties.OfType<PrimaryKey>().Any(x => x.SqlColumns.Any(y => y.SqlColumn == column)))
            {
                sb.Append(".SetPK()");
            }

            if (column.Properties.OfType<Identity>().Any())
            {
                sb.Append(".SetIdentity()");
            }

            var fkOnColumn = column.Table.Properties.OfType<ForeignKey>().FirstOrDefault(fk => fk.ForeignKeyColumns.Any(fkc => fkc.ForeignKeyColumn == column));
            if (fkOnColumn != null)
            {
                // TODO gen AddForeignkeys?
                if (fkOnColumn.ForeignKeyColumns.Count == 1)
                {
                    sb.Append(".SetForeignKeyTo(nameof(")
                       // TODO spec name
                       .Append(fkOnColumn.SqlTable.SchemaAndTableName.TableName)
                       .Append("))");
                }
                else
                {
                    // Only create after last
                    if (column == fkOnColumn.ForeignKeyColumns.Last().ForeignKeyColumn)
                    {
                        sb.AppendLine(";")
                            .Append(3, "table.SetForeignKeyTo(nameof(")
                            .Append(fkOnColumn.ReferredTable.SchemaAndTableName.TableName)
                            .AppendLine("), new List<ColumnReference>()")
                            .AppendLine(3, "{");

                        foreach (var fkColumnMap in fkOnColumn.ForeignKeyColumns)
                        {
                            sb.Append(4, "new ColumnReference(\"").Append(fkColumnMap.ForeignKeyColumn.Name).Append("\", ").Append(fkColumnMap.ReferredColumn.Name).AppendLine("\"),");
                        }

                        sb.Append(3, "})");
                    }
                    // throw new NotImplementedException("Multiple FK columns");
                }
            }

            // TODO Default Value + config

            sb.Append(";");

            var descriptionProperty = column.Properties.OfType<SqlColumnDescription>().FirstOrDefault();
            if (!string.IsNullOrEmpty(descriptionProperty?.Description))
            {
                sb.Append(" // ").Append(descriptionProperty.Description.Replace("\r", string.Empty, StringComparison.OrdinalIgnoreCase).Replace("\n", string.Empty, StringComparison.OrdinalIgnoreCase));
            }

            return sb.ToString();
        }

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
                /*case SqlType.Binary:
                    return "BINARY";
                case SqlType.VarBinary:
                    return "VARBINARY";
                case SqlType.Image :
                    return "IMAGE";
                case SqlType.NText:
                    return "NTEXT";*/
                _ => throw new NotImplementedException($"Unmapped SqlType: {Enum.GetName(typeof(SqlType), column.Type)}"),
            };
        }
    }

    internal static class StringBuilderExtensions
    {
        public static int indentationSpaces = 4;

        public static StringBuilder Append(this StringBuilder sb, int level, string value)
        {
            sb.Append(new string(' ', level * indentationSpaces))
                .Append(value);
            return sb;
        }

        public static StringBuilder AppendLine(this StringBuilder sb, int level, string value)
        {
            sb.Append(new string(' ', level * indentationSpaces))
                .AppendLine(value);
            return sb;
        }
    }
}