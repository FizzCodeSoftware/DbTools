namespace FizzCode.DbTools.DataDefinitionGenerator
{
    using FizzCode.DbTools.DataDefinition.Migration;

    public interface ISqlMigrationGenerator
    {
        ISqlGenerator Generator { get; }
        string CreateTable(TableNew tableNew);
        string DropTable(TableDelete tableDelete);
    }
}