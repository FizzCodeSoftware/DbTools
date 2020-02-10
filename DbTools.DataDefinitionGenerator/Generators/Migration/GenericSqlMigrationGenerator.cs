namespace FizzCode.DbTools.DataDefinitionGenerator
{
    using System;
    using System.Linq;
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

        public virtual string DropColumns(params ColumnDelete[] columnDelete)
        {
            var tableNames = columnDelete.Select(c => c.SqlColumn.Table.SchemaAndTableName).Distinct();

            if (tableNames.Count()
                != 1)
                throw new ArgumentOutOfRangeException(nameof(columnDelete), "All columns should be on the same table.");

            var tableName = tableNames.First();

            var columnsToDelete = columnDelete.Select(c => c.SqlColumn.Name).ToList();
            return @$"
ALTER TABLE {tableName}
DROP COLUMN { string.Join(", ", columnsToDelete) }";
        }

        public string CreateColumn(ColumnNew columnNew)
        {
            throw new NotImplementedException();
        }

        private ISqlGenerator _generator;

        public ISqlGenerator Generator => _generator ?? (_generator = CreateGenerator());
    }
}