namespace FizzCode.DbTools.DataDefinition.Base.Migration
{
    using FizzCode.DbTools.DataDefinition.Base;
    public class TableNew : TableMigration
    {
        public TableNew()
        {
        }

        public TableNew(SqlTable table)
        {
            SchemaAndTableName = table.SchemaAndTableName;
            foreach (var column in table.Columns)
            {
                var newColumn = new SqlColumn
                {
                    Table = this
                };
                Columns.Add(column.CopyTo(newColumn));
            }

            // TODO copy .Properties
        }
        public override string ToString()
        {
            return "(New:) " + base.ToString();
        }
    }
}
