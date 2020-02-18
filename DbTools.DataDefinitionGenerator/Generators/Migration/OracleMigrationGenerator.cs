namespace FizzCode.DbTools.DataDefinitionGenerator
{
    using System;
    using System.Linq;
    using System.Text;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition.Migration;

    public class OracleMigrationGenerator : GenericSqlMigrationGenerator
    {
        public OracleMigrationGenerator(Context context)
            : base(context)
        {
        }

        protected override ISqlGenerator CreateGenerator()
        {
            return new OracleGenerator12c(Context);
        }

        public override string CreateColumns(params ColumnNew[] columnNews)
        {
            var tableNames = columnNews.Select(c => c.SqlColumn.Table.SchemaAndTableName).Distinct();

            if (tableNames.Count() != 1)
                throw new ArgumentOutOfRangeException(nameof(columnNews), "All columns should be on the same table.");

            var tableName = tableNames.First();

            var columnsToAdd = columnNews.Select(c => c.SqlColumn.Name).ToList();

            var sb = new StringBuilder();
            sb.Append("ALTER TABLE ").AppendLine(Generator.GetSimplifiedSchemaAndTableName(tableName));
            sb.Append("ADD (");

            var idx = 0;
            foreach (var columnNew in columnNews)
            {
                if (idx++ > 0)
                    sb.AppendLine(",");

                sb.AppendLine(Generator.GenerateCreateColumn(columnNew.SqlColumn));
            }

            sb.Append(")");

            return sb.ToString();
        }

        public override string DropColumns(params ColumnDelete[] columnDeletes)
        {
            var tableNames = columnDeletes.Select(c => c.SqlColumn.Table.SchemaAndTableName).Distinct();

            if (tableNames.Count() != 1)
                throw new ArgumentOutOfRangeException(nameof(columnDeletes), "All columns should be on the same table.");

            var tableName = tableNames.First();

            var columnsToDelete = columnDeletes.Select(c => Generator.GuardKeywords(c.SqlColumn.Name)).ToList();

            var sb = new StringBuilder();
            sb.Append("ALTER TABLE ").AppendLine(Generator.GetSimplifiedSchemaAndTableName(tableName));

            if (columnsToDelete.Count > 1)
            {
                sb.Append("DROP (");
                var idx = 0;
                foreach (var column in columnsToDelete)
                {
                    if (idx++ > 0)
                        sb.Append(", ");

                    sb.Append(column);
                }

                sb.Append(")");
            }
            else
            {
                sb.Append("DROP COLUMN ");
                sb.Append(columnsToDelete[0]);
            }

            return sb.ToString();
        }
    }
}