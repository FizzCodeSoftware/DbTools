namespace FizzCode.DbTools.DataDefinition.Migration
{
    public class TableNew : TableMigration
    {
        public TableNew()
        {
        }

        public TableNew(SqlTable original)
        {
            SchemaAndTableName = original.SchemaAndTableName;
            foreach (var column in original.Columns)
            {
                var newColumn = new SqlColumn
                {
                    Table = this
                };
                Columns.Add(column.CopyTo(newColumn));
            }

            // TODO copy .Properties
        }
    }
}
