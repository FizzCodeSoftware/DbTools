namespace FizzCode.DbTools.DataDefinition.SqlExecuter
{
    using FizzCode.DbTools.DataDefinition.Migration;
    using FizzCode.DbTools.DataDefinition.SqlGenerator;

    public class DatabaseMigrator : DatabaseTask
    {
        public DatabaseMigrator(SqlStatementExecuter sqlExecuter, ISqlMigrationGenerator migrationGenerator)
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

        public void NewPrimaryKey(PrimaryKeyNew primaryKeyNew)
        {
            var sql = MigrationGenerator.CreatePrimaryKey(primaryKeyNew);
            Executer.ExecuteNonQuery(sql);
        }

        public void NewForeignKey(ForeignKeyNew foreignKeyNew)
        {
            var sql = MigrationGenerator.Generator.CreateForeignKey(foreignKeyNew.ForeignKey);
            Executer.ExecuteNonQuery(sql);
        }
    }
}