namespace FizzCode.DbTools.DataDefinitionGenerator
{
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
            return "";
        }

        public virtual string CreateTable(TableNew tableNew)
        {
            // TODO Properties (PKs, FKs, Indexes, Defaults, Descriptions)
            return Generator.CreateTable(tableNew);
        }

        protected abstract ISqlGenerator CreateGenerator();

        private ISqlGenerator _generator;

        public ISqlGenerator Generator => _generator ?? (_generator = CreateGenerator());
    }
}