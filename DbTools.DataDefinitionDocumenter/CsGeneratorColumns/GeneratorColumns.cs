namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System;
    using System.Linq;
    using System.Text;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;

    public abstract class GeneratorColumns
    {
        public Context Context { get; }
        public SqlVersion Version { get; protected set; }

        protected GeneratorColumns(Context context)
        {
            Context = context;
        }

        public string GetColumnCreation(SqlColumn column)
        {
            var sb = new StringBuilder();

            sb.Append(3, "table.")
                .Append(GetColumnCreationMethod(column));

            sb.Append(IsNullable(column));

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
                sb.Append(" // ").Append(descriptionProperty.Description.Replace("\r", "", StringComparison.OrdinalIgnoreCase).Replace("\n", "", StringComparison.OrdinalIgnoreCase));
            }

            return sb.ToString();
        }

        protected abstract string GetColumnCreationMethod(SqlColumn column);

        protected string IsNullable(SqlColumn column)
        {
            if (column.Types[Version].IsNullable)
                return ", true";

            return "";
        }
    }

    public class GenericGeneratorColumns1 : GeneratorColumns
    {
        public GenericGeneratorColumns1(Context context) : base(context)
        {
            Version = new Generic1();
        }

        protected override string GetColumnCreationMethod(SqlColumn column)
        {
            var type = column.Types[Version];
            return type.SqlTypeInfo.DbType switch
            {
                "CHAR" => $"AddNVarChar(\"{column.Name}\", {type.Length}",
                "NCHAR" => $"AddNChar(\"{column.Name}\", {type.Length}",
                "VARCHAR" => $"AddVarChar(\"{column.Name}\", {type.Length}",
                "NVARCHAR" => $"AddNVarChar(\"{column.Name}\", {type.Length}",
                // TODO "DEFAULTPK"
                "FLOAT_SMALL" => $"AddFloat(\"{column.Name}\"",
                "FLOAT_LARGE" => $"AddReal(\"{column.Name}\"",
                "BIT" => $"AddBit(\"{column.Name}\"",
                "BYTE" => $"AddByte(\"{column.Name}\"",
                "INT16" => $"AddInt16(\"{column.Name}\"",
                "INT32" => $"AddInt32(\"{column.Name}\"",
                "INT64" => $"AddInt64(\"{column.Name}\"",
                "NUMBER" => $"AddNumber(\"{column.Name}\", {type.Length}, {type.Scale}",
                "DATE" => $"AddDate(\"{column.Name}\"",
                "TIME" => $"AddTime(\"{column.Name}\"",
                "DATETIME" => $"AddDateTime(\"{column.Name}\"",
                _ => throw new NotImplementedException($"Unmapped type: {type.SqlTypeInfo.DbType}"),
            };
        }
    }

    public class MsSqlCsGeneratorColumns2016 : GeneratorColumns
    {
        public MsSqlCsGeneratorColumns2016(Context context) : base(context)
        {
            Version = new Generic1();
        }

        protected override string GetColumnCreationMethod(SqlColumn column)
        {
            var type = column.Types[Version];
            return type.SqlTypeInfo.DbType switch
            {
                "CHAR" => $"AddNVarChar(\"{column.Name}\", {type.Length}",
                "NCHAR" => $"AddNChar(\"{column.Name}\", {type.Length}",
                "VARCHAR" => $"AddVarChar(\"{column.Name}\", {type.Length}",
                "NVARCHAR" => $"AddNVarChar(\"{column.Name}\", {type.Length}",
                "NTEXT" => $"AddNText(\"{column.Name}\"",
                // TODO "DEFAULTPK"
                "FLOAT" => $"AddFloat(\"{column.Name}\"",
                "REAL" => $"AddReal(\"{column.Name}\"",
                "BIT" => $"AddBit(\"{column.Name}\"",
                "SMALLINT" => $"AddByte(\"{column.Name}\"",
                "TINYINT" => $"AddInt16(\"{column.Name}\"",
                "INT" => $"AddInt32(\"{column.Name}\"",
                "BIGINT" => $"AddInt64(\"{column.Name}\"",
                "DECIMAL" => $"AddDecimal(\"{column.Name}\", {type.Length}, {type.Scale}",
                "NUMERIC" => $"AddNumeric(\"{column.Name}\", {type.Length}, {type.Scale}",
                "MONEY" => $"AddMoney(\"{column.Name}\"",
                "SMALLMONEY" => $"AddSmallMoney(\"{column.Name}\"",
                "DATE" => $"AddDate(\"{column.Name}\"",
                "TIME" => $"AddTime(\"{column.Name}\", {type.Length}",
                "DATETIME" => $"AddDateTime(\"{column.Name}\"",
                "DATETIME2" => $"AddDateTime2(\"{column.Name}\", {type.Length}",
                "DATETIMEOFFSET" => $"AddDateTime2(\"{column.Name}\", {type.Length}",
                "SMALLDATETIME" => $"AddSmallDateTime(\"{column.Name}\"",
                "BINARY" => $"AddBinary(\"{column.Name}\", {type.Length}",
                "VARBINARY" => $"AddVarBinary(\"{column.Name}\", {type.Length}",
                "IMAGE" => $"AddImage(\"{column.Name}\"",
                "XML" => $"AddXml(\"{column.Name}\"",
                _ => throw new NotImplementedException($"Unmapped type: {type.SqlTypeInfo.DbType}"),
            };
        }
    }

    public class OracleCsGeneratorColumns12c : GeneratorColumns
    {
        public OracleCsGeneratorColumns12c(Context context) : base(context)
        {
            Version = new Oracle12c();
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

    public class SqLiteCsGeneratorColumns3: GeneratorColumns
    {
        public SqLiteCsGeneratorColumns3(Context context) : base(context)
        {
            Version = new SqLite3();
        }

        protected override string GetColumnCreationMethod(SqlColumn column)
        {
            var type = column.Types[Version];
            return type.SqlTypeInfo.DbType switch
            {
                "INTEGER" => $"AddNVarChar(\"{column.Name}\"",
                "REAL" => $"AddNVarChar(\"{column.Name}\"",
                "TEXT" => $"AddNVarChar(\"{column.Name}\"",
                "BLOB" => $"AddNVarChar(\"{column.Name}\"",
                _ => throw new NotImplementedException($"Unmapped type: {type.SqlTypeInfo.DbType}"),
            };
        }
    }
}