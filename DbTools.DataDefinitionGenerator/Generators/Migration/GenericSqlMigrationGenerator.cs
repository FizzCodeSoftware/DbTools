namespace FizzCode.DbTools.DataDefinitionGenerator
{
    using System;
    using System.Linq;
    using System.Text;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Migration;

    public abstract class GenericSqlMigrationGenerator : ISqlMigrationGenerator
    {
        public Context Context { get;  }

        protected GenericSqlMigrationGenerator(Context context)
        {
            Context = context;
        }

        public virtual string DropTable(TableDelete tableDelete)
        {
            // Remove indexes, Remove FKs
            // + normal DropTable

            return Generator.DropTable(tableDelete);
        }

        public virtual string RenameTable(TableRename tableRename)
        {
            throw new NotImplementedException();
        }

        public virtual string CreateTable(TableNew tableNew)
        {
            // TODO Properties (PKs, FKs, Indexes, Defaults, Descriptions)
            return Generator.CreateTable(tableNew);
        }

        protected abstract ISqlGenerator CreateGenerator();

        public virtual string DropColumns(params ColumnDelete[] columnDeletes)
        {
            var tableName = CheckSameTable(columnDeletes);

            var columnsToDelete = columnDeletes.Select(c => Generator.GuardKeywords(c.SqlColumn.Name)).ToList();
            return @$"
ALTER TABLE {Generator.GetSimplifiedSchemaAndTableName(tableName)}
DROP COLUMN { string.Join(", ", columnsToDelete) }";
        }

        public virtual string CreateColumns(params ColumnNew[] columnNews)
        {
            var tableName = CheckSameTable(columnNews);

            var columnsToAdd = columnNews.Select(c => c.SqlColumn.Name).ToList();

            var sb = new StringBuilder();
            sb.AppendLine($"ALTER TABLE {Generator.GetSimplifiedSchemaAndTableName(tableName)}");
            sb.Append("ADD ");

            var idx = 0;
            foreach (var columnNew in columnNews)
            {
                if (idx++ > 0)
                    sb.AppendLine(",");

                sb.AppendLine(Generator.GenerateCreateColumn(columnNew.SqlColumn));
            }

            return sb.ToString();
        }

        private static SchemaAndTableName CheckSameTable(ColumnMigration[] columnNews)
        {
            var tableNames = columnNews.Select(c => c.SqlColumn.Table.SchemaAndTableName).Distinct();

            if (tableNames.Count()
                != 1)
                throw new ArgumentOutOfRangeException(nameof(columnNews), "All columns should be on the same table.");
            
            return tableNames.First();
        }

        public SqlStatementWithParameters ChangeColumns(params ColumnChange[] columnChanges)
        {
            var tableName = CheckSameTable(columnChanges);

            if (columnChanges.Length == 1)
            {
                return $@"
ALTER TABLE {Generator.GetSimplifiedSchemaAndTableName(tableName)}
ALTER COLUMN {Generator.GenerateCreateColumn(columnChanges[0].NewNameAndType)}";
            }
            else
            {
                var sbStatements = new StringBuilder();
                // TODO Oprions ShouldMigrateColumnChangesAllAtOnce
                // TODO multiple -> temp table
                // TODO drop constraints then re add them
                foreach (var columnChange in columnChanges)
                {
                    sbStatements.AppendLine(ChangeColumns(columnChange).Statement);
                }

                return new SqlStatementWithParameters(sbStatements.ToString());
            }
        }

        private ISqlGenerator _generator;

        public ISqlGenerator Generator => _generator ?? (_generator = CreateGenerator());
    }
}