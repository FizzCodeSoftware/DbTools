namespace FizzCode.DbTools.DataDefinition.Migration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.Common;

    public class Comparer
    {
        public Context Context { get; }

        public Comparer(Context context)
        {
            Context = context;
        }

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

            return changes;
        }
    }
}
