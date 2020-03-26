namespace FizzCode.DbTools.DataDefinition.Migration
{
    using System.Collections.Generic;
    using System.Linq;

    public static class ComparerIndex
    {
        public static List<IndexMigration> CompareIndexes(SqlTable tableOriginal, SqlTable tableNew)
        {
            var changes = new List<IndexMigration>();

            foreach (var indexOriginal in tableOriginal.Properties.OfType<Index>())
            {
                if (!tableNew.Properties.OfType<Index>().Any(indexNew => indexNew.Name == indexOriginal.Name))
                {
                    changes.Add(new IndexDelete()
                    {
                        Index = indexOriginal
                    });
                }
            }

            // TODO detect name change? (same index elements but different name?)
            foreach (var indexNew in tableNew.Properties.OfType<Index>())
            {
                var indexOriginal = tableOriginal.Properties.OfType<Index>().FirstOrDefault(indexOriginal => indexOriginal.Name == indexNew.Name);
                if (indexOriginal == null)
                {
                    changes.Add(new IndexNew()
                    {
                        Index = indexNew
                    });
                }
                else
                {
                    var indexChanged = false;
                    var indexChange = new IndexChange()
                    {
                        Index = indexOriginal,
                        NewIndex = indexNew
                    };

                    // compare referred tables
                    // compare columns
                    if (indexOriginal.SqlTable.SchemaAndTableName != indexNew.SqlTable.SchemaAndTableName)
                        indexChanged = true;

                    if (!CompareIndexColumns(indexOriginal, indexNew))
                    {
                        indexChanged = true;
                        //IndexChange. = new ForeignKeyInternalColumnChanges();
                    }

                    if (indexChanged)
                        changes.Add(indexChange);
                }
            }

            return changes;
        }

        private static bool CompareIndexColumns(Index indexOriginal, Index indexNew)
        {
            if (indexOriginal.SqlColumns.Count != indexNew.SqlColumns.Count)
                return false;

            if (indexOriginal.Includes.Count != indexNew.Includes.Count)
                return false;

            for (var i = 0; i < indexOriginal.SqlColumns.Count; i++)
            {
                if (indexOriginal.SqlColumns[i].Order != indexNew.SqlColumns[i].Order)
                    return false;

                if (Comparer.ColumnChanged(indexOriginal.SqlColumns[i].SqlColumn, indexNew.SqlColumns[i].SqlColumn))
                    return false;
            }

            for (var i = 0; i < indexOriginal.Includes.Count; i++)
            {
                if (indexOriginal.SqlColumns[i].Order != indexNew.SqlColumns[i].Order)
                    return false;

                if (indexOriginal.SqlColumns[i].SqlColumn.Name != indexNew.SqlColumns[i].SqlColumn.Name)
                    return false;

                if (Comparer.ColumnChanged(indexOriginal.SqlColumns[i].SqlColumn, indexNew.SqlColumns[i].SqlColumn))
                    return false;
            }

            return true;
        }
    }
}
