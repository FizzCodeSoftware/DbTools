using FizzCode.DbTools.DataDefinition.Base.Migration;

namespace FizzCode.DbTools.Interfaces;
public interface IDatabaseMigrator
{
    void ChangeColumns(params ColumnChange[] columnChanges);
    void CreateColumns(params ColumnNew[] columnNews);
    void DeleteColumns(params ColumnDelete[] columnDeletes);
    void DeleteTable(TableDelete tableDelete);
    void NewForeignKey(ForeignKeyNew foreignKeyNew);
    void NewPrimaryKey(PrimaryKeyNew primaryKeyNew);
    void NewTable(TableNew tableNew);
}