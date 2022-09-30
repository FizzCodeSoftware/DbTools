namespace FizzCode.DbTools.DataDefinition.Base.Migration
{
    using FizzCode.DbTools.DataDefinition.Base;

    public abstract class ColumnMigration : IMigration
    {
        public SqlColumn SqlColumn { get; set; }

        public override string ToString()
        {
            return SqlColumn.ToString();
        }
    }
}
