namespace FizzCode.DbTools.DataDefinition.Migration
{
    using System;
    using System.Collections.Generic;
    using FizzCode.DbTools.Common;

    public class Comparer
    {
        public Context Context { get; }

        public Comparer(Context context)
        {
            Context = context;
        }

#pragma warning disable CA1822 // Mark members as static
        public List<object> Compare(DatabaseDefinition originalDd, DatabaseDefinition newDd)
#pragma warning restore CA1822 // Mark members as static
        {
            // TODO needs to be ordered
            var changes = new List<object>();

            // Compare tables
            // handle renamed tables - needs parameter / external info
            // detect deleted tables
            // detect new tables
            foreach (var tableOriginal in originalDd.GetTables())
            {
                if (!newDd.Contains(tableOriginal.SchemaAndTableName))
                {
                    var tableDelete = new TableDelete
                    {
                        SchemaAndTableName = tableOriginal.SchemaAndTableName
                    };

                    changes.Add(tableDelete);
                }
            }

            foreach (var tableNewDd in newDd.GetTables())
            {
                if (!originalDd.Contains(tableNewDd.SchemaAndTableName))
                {
                    var tableNew = new TableNew(tableNewDd);
                    changes.Add(tableNew);
                }
            }

            foreach (var tableOriginal in originalDd.GetTables())
            {
                // not deleted
                if (newDd.Contains(tableOriginal.SchemaAndTableName))
                {
                    var tableNew = newDd.Tables[tableOriginal.SchemaAndTableName];
                    changes.AddRange(CompareColumns(tableOriginal, tableNew));
                }
            }

            return changes;
        }

        private List<object> CompareColumns(SqlTable tableOriginal, SqlTable tableNew)
        {
            var changes = new List<object>();
            foreach (var columnOriginal in tableOriginal.Columns)
            {
                tableNew.Columns.TryGetValue(columnOriginal.Name, out var columnNew);
                if (columnNew == null)
                {
                    var columnDelete = new ColumnDelete
                    {
                        SqlColumn = columnOriginal.CopyTo(new SqlColumn())
                    };
                    changes.Add(columnDelete);
                }
            }

            foreach (var columnNew in tableNew.Columns)
            {
                tableOriginal.Columns.TryGetValue(columnNew.Name, out var columnOriginal);
                if (columnOriginal == null)
                {
                    var column = new ColumnNew
                    {
                        SqlColumn = columnNew.CopyTo(new SqlColumn())
                    };
                    changes.Add(column);
                }
            }

            return changes;
        }
    }
}
