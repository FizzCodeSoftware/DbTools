namespace FizzCode.DbTools.DataDefinitionGenerator
{
    using System;
    using System.Linq;
    using System.Text;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition.Migration;

    public abstract class GenericSqlMigrationGenerator : ISqlMigrationGenerator
    {
        public Context Context { get; }

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
            var tableNames = columnDeletes.Select(c => c.SqlColumn.Table.SchemaAndTableName).Distinct();

            if (tableNames.Count() != 1)
                throw new ArgumentOutOfRangeException(nameof(columnDeletes), "All columns should be on the same table.");

            var tableName = tableNames.First();

            var columnsToDelete = columnDeletes.Select(c => Generator.GuardKeywords(c.SqlColumn.Name)).ToList();
            return @$"
ALTER TABLE {Generator.GetSimplifiedSchemaAndTableName(tableName)}
DROP COLUMN { string.Join(", ", columnsToDelete) }";
        }

        public virtual string CreateColumns(params ColumnNew[] columnNews)
        {
            var tableNames = columnNews.Select(c => c.SqlColumn.Table.SchemaAndTableName).Distinct();

            if (tableNames.Count() != 1)
                throw new ArgumentOutOfRangeException(nameof(columnNews), "All columns should be on the same table.");

            var tableName = tableNames.First();

            var columnsToAdd = columnNews.Select(c => c.SqlColumn.Name).ToList();

            var sb = new StringBuilder();
            sb.Append("ALTER TABLE ").AppendLine(Generator.GetSimplifiedSchemaAndTableName(tableName));
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

        private ISqlGenerator _generator;

        public ISqlGenerator Generator => _generator ?? (_generator = CreateGenerator());
    }
}