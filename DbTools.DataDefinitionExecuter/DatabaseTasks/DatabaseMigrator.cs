namespace FizzCode.DbTools.DataDefinitionExecuter
{
    using FizzCode.DbTools.DataDefinition.Migration;
    using FizzCode.DbTools.DataDefinitionGenerator;

    public class DatabaseMigrator : DatabaseTask
    {
        public DatabaseMigrator(SqlExecuter sqlExecuter, ISqlMigrationGenerator migrationGenerator)
            : base(sqlExecuter)
        {
            MigrationGenerator = migrationGenerator;
        }

        protected ISqlMigrationGenerator MigrationGenerator { get; }

        public void NewTable(TableNew tableNew)
        {
            var sql = MigrationGenerator.CreateTable(tableNew);
            Executer.ExecuteNonQuery(sql);
        }

        public void DeleteTable(TableDelete tableDelete)
        {
            var sql = MigrationGenerator.DropTable(tableDelete);
            Executer.ExecuteNonQuery(sql);
        }

        public void DeleteColumns(params ColumnDelete[] columnDeletes)
        {
            var sql = MigrationGenerator.DropColumns(columnDeletes);
            Executer.ExecuteNonQuery(sql);
        }

        public void CreateColumns(params ColumnNew[] columnNews)
        {
            var sql = MigrationGenerator.CreateColumns(columnNews);
            Executer.ExecuteNonQuery(sql);
        }

        public void ChangeColumns(params ColumnChange[] columnChanges)
        {
            var sql = MigrationGenerator.ChangeColumns(columnChanges);
            Executer.ExecuteNonQuery(sql);
        }
    }
}