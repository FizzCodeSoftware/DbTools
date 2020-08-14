namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;

    public class CSharpGenerator : AbstractCSharpGenerator
    {
        public CSharpGenerator(AbstractCSharpWriter writer, SqlEngineVersion version, string databaseName, string @namespace)
            : base(writer, version, databaseName, @namespace)
        {
        }

        protected override void GenerateTable(StringBuilder sb, SqlTable table)
        {
            var pks = table.Properties.OfType<PrimaryKey>().ToList();
            if (pks.Count == 0)
            {
                sb.AppendLine(2, "// no primary key");
            }

            var tableComment = OnTableComment?.Invoke(table);
            if (!string.IsNullOrEmpty(tableComment))
            {
                sb.Append(2, "// ").AppendLine(tableComment);
            }

            sb
                .Append(2, "public SqlTable ")
                .Append(Helper.GetSimplifiedSchemaAndTableName(table.SchemaAndTableName, DatabaseDeclaration.SchemaTableNameSeparator.ToString(CultureInfo.InvariantCulture)))
                .AppendLine(" { get; } = AddTable(table =>")
                .AppendLine(2, "{");

            var tableAnnotation = OnTableAnnotation?.Invoke(table);
            if (!string.IsNullOrEmpty(tableAnnotation))
            {
                sb.Append(3, "table").Append(tableAnnotation).AppendLine(";");
            }

            var pkColumns = table.Columns
                .Where(column => column.Table.Properties.OfType<PrimaryKey>().Any(x => x.SqlColumns.Any(y => y.SqlColumn == column)))
                .ToList();

            foreach (var column in pkColumns)
            {
                var columnAnnotation = OnColumnAnnotation?.Invoke(column);
                var columnComment = OnColumnComment?.Invoke(column);

                var columnCreation = Writer.GetColumnCreation(column, Helper, columnAnnotation, columnComment);
                sb.AppendLine(columnCreation);
            }

            var regularColumns = table.Columns
                .Where(x => !pkColumns.Contains(x))
                .ToList();

            foreach (var column in regularColumns)
            {
                var columnAnnotation = OnColumnAnnotation?.Invoke(column);
                var columnComment = OnColumnComment?.Invoke(column);

                var columnCreation = Writer.GetColumnCreation(column, Helper, columnAnnotation, columnComment);
                sb.AppendLine(columnCreation);
            }

            GenerateTableProperties(sb, table);
            sb.AppendLine(2, "});");
        }

#pragma warning disable CA1308 // Normalize strings to uppercase
        protected override void GenerateIndex(StringBuilder sb, Index index)
        {
            // TODO clustered

            sb.Append(3, "table.AddIndexWithName(");
            if (index.Includes.Count == 0)
            {
                sb.Append(index.Clustered.ToString().ToLowerInvariant())
                    .Append(", ");

                sb.Append('"')
                    .Append(index.Name)
                    .Append("\", ");

                sb.Append(string.Join(", ", index.SqlColumns.Select(i => "\"" + i.SqlColumn.Name + "\"").ToList()));
            }
            else
            {
                sb.Append(index.Clustered.ToString().ToLowerInvariant())
                    .Append(", ");

                sb.Append('"')
                    .Append(index.Name)
                    .Append("\", ");

                sb.Append("new [] {")
                    .Append(string.Join(", ", index.SqlColumns.Select(i => "\"" + i.SqlColumn.Name + "\"").ToList()))
                    .Append("}, ")
                    .Append("new [] {")
                    .Append(string.Join(", ", index.Includes.Select(i => "\"" + i.Name + "\"").ToList()))
                    .Append('}');
            }

            sb.AppendLine(");");
        }
#pragma warning restore CA1308 // Normalize strings to uppercase

        protected override void GenerateUniqueConstraint(StringBuilder sb, UniqueConstraint uniqueConstraint)
        {
            sb.Append(3, "table.AddUniqueConstraintWithName(");

            sb.Append('"');
            sb.Append(uniqueConstraint.Name);
            sb.Append("\", ");

            sb.Append(string.Join(", ", uniqueConstraint.SqlColumns.Select(c => "\"" + c.SqlColumn.Name + "\"").ToList()));

            sb.AppendLine(");");
        }

        protected override void WriteSingleFileFooter(StringBuilder sb)
        {
            sb.AppendLine(1, "}")
                .Append('}');
        }

        protected override void WriteSingleFileHeader(StringBuilder sb, List<SqlTable> tables, bool partialClass = false)
        {
            WriteSingleFileHeaderCommon(sb, partialClass);
        }

        protected override void GenerateCustomTableProperties(StringBuilder sb, SqlTableCustomProperty customProperty)
        {
        }
    }
}
