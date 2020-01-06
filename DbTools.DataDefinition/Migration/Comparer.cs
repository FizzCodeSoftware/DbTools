namespace FizzCode.DbTools.DataDefinition.Migration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Comparer
    {
        public List<object> Compare(DatabaseDefinition originalDd, DatabaseDefinition newDd)
        {
            // TODO needs to be ordered
            var changes = new List<object>();

            // Compare tables
            // handle renamed tables - needs parameter / external info
            // detect deleted tables
            // detect new tables
            foreach (var tableOriginal in originalDd.GetTables())
            {
                var tableNew = newDd.GetTable(tableOriginal.SchemaAndTableName);
                if (tableNew == null)
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
                var tableOriginal = originalDd.GetTable(tableNewDd.SchemaAndTableName);

                if (tableOriginal == null)
                {
                    var tableNew = new TableNew(tableOriginal);
                    changes.Add(tableNew);
                }
            }

            return changes;
        }
    }
}
