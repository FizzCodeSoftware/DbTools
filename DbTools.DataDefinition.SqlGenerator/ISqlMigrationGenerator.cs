namespace FizzCode.DbTools.DataDefinition.SqlGenerator
{
    using FizzCode.DbTools.DataDefinition.Migration;

    public interface ISqlMigrationGenerator
    {
        ISqlGenerator Generator { get; }
        string CreateTable(TableNew tableNew);
        string DropTable(TableDelete tableDelete);

        string DropColumns(params ColumnDelete[] columnDeletes);
        string CreateColumns(params ColumnNew[] columnNews);

        SqlStatementWithParameters ChangeColumns(params ColumnChange[] columnChanges);

        string CreatePrimaryKey(PrimaryKeyNew primaryKeyNew);
    }
}