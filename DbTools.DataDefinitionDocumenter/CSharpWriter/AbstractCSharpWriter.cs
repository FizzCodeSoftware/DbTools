namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System;
    using System.Linq;
    using System.Text;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;

    public abstract class AbstractCSharpWriter
    {
        public DocumenterContext Context { get; }
        public SqlEngineVersion Version { get; }
        public Type TypeMapperType { get; }

        protected AbstractCSharpWriter(DocumenterContext context, SqlEngineVersion version, Type typeMapperType)
        {
            Context = context;
            Version = version;
            TypeMapperType = typeMapperType;
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
                if (Context.Customizer.ShouldSkip(fkOnColumn.ReferredTable.SchemaAndTableName))
                {
                    sb.Append("; //");
                }

                // TODO gen AddForeignkeys?
                if (fkOnColumn.ForeignKeyColumns.Count == 1)
                {
                    sb.Append(".SetForeignKeyTo(nameof(")
                       // TODO spec name
                       .Append(fkOnColumn.ReferredTable.SchemaAndTableName.TableName)
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

        public virtual string GetSqlTypeNamespace()
        {
            return $"FizzCode.DbTools.DataDefinition.{Version}";
        }
    }
}