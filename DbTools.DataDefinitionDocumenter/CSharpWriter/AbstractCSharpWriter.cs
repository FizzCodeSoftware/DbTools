using System;
using System.Globalization;
using System.Linq;
using System.Text;
using FizzCode.DbTools.Common;
using FizzCode.DbTools.DataDefinition;
using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDefinitionDocumenter;
public abstract class AbstractCSharpWriter : AbstractCSharpWriterBase
{
    protected AbstractCSharpWriter(GeneratorContext context, SqlEngineVersion version, Type typeMapperType, string databaseName)
        : base(context, version, typeMapperType, databaseName)
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

        sb.Append(3, "table.")
            .Append(GetColumnCreationMethod(column))
            .Append(IsNullable(column))
            .Append(')');

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

        sb.Append(';');

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
                sb.Append(' ').Append(comment);
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
            .Append(3, "table.SetForeignKeyTo(nameof(")
            .Append(helper.GetSimplifiedSchemaAndTableName(fkOnColumn.ReferredTable.SchemaAndTableName, DatabaseDeclarationConst.SchemaTableNameSeparator.ToString(CultureInfo.InvariantCulture)))
            .AppendLine("), ");

        sb.Append("new []")
            .AppendLine(4, "{");

        foreach (var fkColumnMap in fkOnColumn.ForeignKeyColumns)
        {
            sb.Append(5, "new ColumnReference(\"")
                .Append(fkColumnMap.ForeignKeyColumn.Name)
                .Append("\", \"")
                .Append(fkColumnMap.ReferredColumn.Name)
                .AppendLine("\"),");
        }

        sb.Append(3, "})");
    }
}