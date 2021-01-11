namespace FizzCode.DbTools.DataDefinition.Migration
{
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

        public List<IMigration> Compare(DatabaseDefinition originalDd, DatabaseDefinition newDd)
        {
            // TODO needs to be ordered
            var changes = new List<IMigration>();

            // Compare tables
            // handle renamed tables - needs parameter / external info
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
                    changes.AddRange(ComparerPrimaryKey.ComparePrimaryKeys(tableOriginal, tableNew));
                    changes.AddRange(ComparerForeignKey.CompareForeignKeys(tableOriginal, tableNew));
                    changes.AddRange(ComparerIndex.CompareIndexes(tableOriginal, tableNew));
                    changes.AddRange(ComparerUniqueConstraint.CompareUniqueConstraints(tableOriginal, tableNew));
                }
            }

            return changes;
        }

        private static List<IMigration> CompareColumns(SqlTable tableOriginal, SqlTable tableNew)
        {
            var changes = new List<IMigration>();
            foreach (var columnOriginal in tableOriginal.Columns)
            {
                tableNew.Columns.TryGetValue(columnOriginal.Name, out var columnNew);
                if (columnNew == null)
                {
                    changes.Add(new ColumnDelete()
                    {
                        SqlColumn = columnOriginal
                    });
                }
            }

            foreach (var columnNew in tableNew.Columns)
            {
                tableOriginal.Columns.TryGetValue(columnNew.Name, out var columnOriginal);
                if (columnOriginal == null)
                {
                    changes.Add(new ColumnNew()
                    {
                        SqlColumn = columnNew
                    });
                }
                else
                {
                    var columnChange = new ColumnChange()
                    {
                        SqlColumn = columnOriginal,
                        NewNameAndType = columnNew
                    };

                    var propertyChanges = ComparerIdentity.CompareIdentity(columnOriginal, columnNew);

                    if (propertyChanges.Any()
                        || ColumnChanged(columnNew, columnOriginal))
                    {
                        columnChange.NewNameAndType = columnNew.CopyTo(new SqlColumn());
                        columnChange.SqlColumnPropertyMigrations.AddRange(propertyChanges);
                        changes.Add(columnChange);
                    }
                }
            }

            return changes;
        }

        public static bool ColumnChanged(SqlColumn columnNew, SqlColumn columnOriginal)
        {
            return (columnOriginal.Type.SqlTypeInfo.HasLength && columnOriginal.Type.Length != columnNew.Type.Length)
                                 || (columnOriginal.Type.SqlTypeInfo.HasScale && columnOriginal.Type.Scale != columnNew.Type.Scale)
                                 || columnOriginal.Type.SqlTypeInfo.GetType().Name != columnNew.Type.SqlTypeInfo.GetType().Name
                                 || columnOriginal.Type.IsNullable != columnNew.Type.IsNullable;
        }
    }
}
