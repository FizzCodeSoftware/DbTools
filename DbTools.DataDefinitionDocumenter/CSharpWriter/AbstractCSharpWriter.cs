namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;

    public abstract class AbstractCSharpWriter
    {
        public GeneratorContext GeneratorContext { get; }
        public SqlEngineVersion Version { get; }
        public Type TypeMapperType { get; }

        protected AbstractCSharpWriter(GeneratorContext context, SqlEngineVersion version, Type typeMapperType)
        {
            GeneratorContext = context;
            Version = version;
            TypeMapperType = typeMapperType;
        }

        public string GetColumnCreation(SqlColumn column, DocumenterHelper helper, string extraAnnotation, string comment)
        {
            var sb = new StringBuilder();

            if (GeneratorContext.GeneratorSettings.ShouldCommentOutColumnsWithFkReferencedTables
                && IsForeignKeyReferencedTableSkipped(column))
            {
                sb.Append("// ");
            }

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

            if (!GeneratorContext.GeneratorSettings.NoForeignKeys)
                AddForeignKeySettings(column, sb, helper);

            // TODO Default Value + config

            if (!string.IsNullOrEmpty(extraAnnotation))
            {
                sb.Append(extraAnnotation);
            }

            sb.Append(";");

            var descriptionProperty = column.Properties.OfType<SqlColumnDescription>().FirstOrDefault();
            var description = descriptionProperty?.Description
                ?.Replace("\r", "", StringComparison.OrdinalIgnoreCase)
                ?.Replace("\n", "", StringComparison.OrdinalIgnoreCase)
                ?.Trim();

            if (!string.IsNullOrEmpty(description))
            {
                sb.Append(" // ").Append(description);

                if (!string.IsNullOrEmpty(comment))
                {
                    sb.Append(" ").Append(comment);
                }
            }
            else if (!string.IsNullOrEmpty(comment))
            {
                sb.Append(" // ").Append(comment);
            }

            return sb.ToString();
        }

        private void AddForeignKeySettings(SqlColumn column, StringBuilder sb, DocumenterHelper helper)
        {
            var fkOnColumn = column.Table.Properties.OfType<ForeignKey>().FirstOrDefault(fk => fk.ForeignKeyColumns.Any(fkc => fkc.ForeignKeyColumn == column));
            if (fkOnColumn != null)
            {
                if (GeneratorContext.GeneratorSettings.SholdCommentOutFkReferences
                    && GeneratorContext.Customizer.ShouldSkip(fkOnColumn.ReferredTable.SchemaAndTableName))
                {
                    sb.Append("; //");
                }

                // TODO gen AddForeignkeys?
                if (fkOnColumn.ForeignKeyColumns.Count == 1)
                {
                    sb.Append(".SetForeignKeyTo(nameof(")
                       // TODO spec name
                       .Append(helper.GetSimplifiedSchemaAndTableName(fkOnColumn.ReferredTable.SchemaAndTableName, DatabaseDeclaration.SchemaTableNameSeparator.ToString(CultureInfo.InvariantCulture)))
                       .Append("))");
                }
                else
                {
                    // Only create after last
                    if (column == fkOnColumn.ForeignKeyColumns.Last().ForeignKeyColumn)
                    {
                        sb.AppendLine(";")
                            .Append(3, "table.SetForeignKeyTo(nameof(")
                            .Append(helper.GetSimplifiedSchemaAndTableName(fkOnColumn.ReferredTable.SchemaAndTableName, DatabaseDeclaration.SchemaTableNameSeparator.ToString(CultureInfo.InvariantCulture)))
                            .AppendLine("), new List<ColumnReference>()")
                            .AppendLine(3, "{");

                        foreach (var fkColumnMap in fkOnColumn.ForeignKeyColumns)
                        {
                            sb.Append(4, "new ColumnReference(\"").Append(fkColumnMap.ForeignKeyColumn.Name).Append("\", ").Append(fkColumnMap.ReferredColumn.Name).AppendLine("\"),");
                        }

                        sb.Append(3, "})");
                    }
                }
            }
        }

        protected bool IsForeignKeyReferencedTableSkipped(SqlColumn column)
        {
            var fkOnColumn = column.Table.Properties.OfType<ForeignKey>().FirstOrDefault(fk => fk.ForeignKeyColumns.Any(fkc => fkc.ForeignKeyColumn == column));

            if (fkOnColumn == null)
                return false;

            return GeneratorContext.Customizer.ShouldSkip(fkOnColumn.ReferredTable.SchemaAndTableName);
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