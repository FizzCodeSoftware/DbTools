namespace FizzCode.DbTools.DataDefinition.Migration
{
#pragma warning disable CA1040 // Avoid empty interfaces
    public interface IMigration
#pragma warning restore CA1040 // Avoid empty interfaces
    { }

    public abstract class ColumnMigration : IMigration
    {
        public SqlColumn SqlColumn { get; set; }

        public override string ToString()
        {
            return SqlColumn.ToString();
        }
    }

    public abstract class TableMigration : SqlTable, IMigration
    {
    }

    public class ColumnChange : ColumnMigration
    {
        public SqlColumn NewNameAndType { get; set; }

        public override string ToString()
        {
            // TODO
            return base.ToString();
        }
    }

    public class ColumnDelete : ColumnMigration
    {
    }

    public class ColumnNew : ColumnMigration
    {
    }

    public class TableRename : TableMigration
    {
        public string NewName { get; set; }
    }

    public class TableDelete : TableMigration
    { }

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

    // Other / all cases
    // New PK, Deleted OK, Renamed PK
    // New FK, Deleted FK, Renamed FK
    // New Index, Deleted Index, Renamed Index

    // New TableDescription, Deleted TableDescription
    // New ColumnDescription, Deleted ColumnDescription

    // New DefaultValue, Deleted DefaultValue
    // New Identity, Deleted Identity

    // (not implemented) Trigger

    // SuspectedTableRename - if remove/add table but the internal schema is exactly the asame, suspect table rename
}
