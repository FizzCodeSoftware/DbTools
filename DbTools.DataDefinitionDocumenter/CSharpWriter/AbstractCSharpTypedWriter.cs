namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;

    public abstract class AbstractCSharpTypedWriter : AbstractCSharpWriterBase
    {
        protected AbstractCSharpTypedWriter(GeneratorContext context, SqlEngineVersion version, Type typeMapperType)
            : base(context, version, typeMapperType)
        {
        }

        public override string GetColumnCreation(SqlColumn column, DocumenterHelper helper, string extraAnnotation, string comment)
        {
            var sb = new StringBuilder();

            if (GeneratorContext.GeneratorSettings.ShouldCommentOutColumnsWithFkReferencedTables
                && IsForeignKeyReferencedTableSkipped(column))
            {
                sb.Append("// ");
            }

            sb
                .Append(2, "public SqlColumn ")
                .Append(column.Name)
                .Append(" { get; } = ")
                .Append(GetColumnCreationMethod(column))
                .Append(IsNullable(column));

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

        protected override void AddForeignKeySettingsMultiColumn(StringBuilder sb, DocumenterHelper helper, ForeignKey fkOnColumn)
        {
            sb.AppendLine(";")
                .AppendLine("")
                .AppendLine("#pragma warning disable IDE1006 // Naming Styles")
                .Append(2, "public ForeignKey ")
                .Append(fkOnColumn.Name)
                .Append(" { get; } = ")
                .Append(Version)
                .Append(".SetForeignKeyTo(nameof(")
                .Append(helper.GetSimplifiedSchemaAndTableName(fkOnColumn.ReferredTable.SchemaAndTableName, DatabaseDeclaration.SchemaTableNameSeparator.ToString(CultureInfo.InvariantCulture)))
                .Append("), ")
                .AppendLine("new []")
                .AppendLine("#pragma warning restore IDE1006 // Naming Styles")
                .AppendLine(3, "{");

            foreach (var fkColumnMap in fkOnColumn.ForeignKeyColumns)
            {
                sb.Append(4, "new ColumnReference(")
                    .Append("nameof(")
                    .Append(fkColumnMap.ForeignKeyColumn.Name)
                    .Append("), nameof(")
                    .Append(fkColumnMap.ReferredColumn.Table.SchemaAndTableName)
                    .Append(".")
                    .Append(fkColumnMap.ReferredColumn.Name)
                    .AppendLine(")),");
            }

            sb.Append(3, "})");
        }
    }
}