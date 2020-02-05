﻿namespace FizzCode.DbTools.DataDefinition.Migration
{
    public class ColumnChange
    {
        public SqlColumn SqlColumn { get; set; }
        public SqlColumn NewNameAndType { get; set; }
    }

    public class ColumnDelete
    {
        public SqlColumn SqlColumn { get; set; }
    }

    public class ColumnNew
    {
        public SqlColumn SqlColumn { get; set; }
    }

    public class TableRename
    {
        public SqlTable SqlTable { get; set; }
        public string NewName { get; set; }
    }

    public class TableDelete : SqlTable
    { }

    public class TableNew : SqlTable
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
}