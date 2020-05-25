namespace FizzCode.DbTools.DataDefinition.Migration
{
    using System.Collections.Generic;
    using System.Linq;

    public abstract class ComparerIndexBase<TIndex, TMigration>
        where TIndex : IndexBase
        where TMigration : IMigration
    {
        public List<TMigration> CompareIndexes(SqlTable tableOriginal, SqlTable tableNew)
        {
            var changes = new List<TMigration>();

            foreach (var indexOriginal in tableOriginal.Properties.OfType<TIndex>())
            {
                if (!tableNew.Properties.OfType<TIndex>().Any(indexNew => indexNew.Name == indexOriginal.Name))
                {
                    changes.Add(CreateDelete(indexOriginal));
                }
            }

            // TODO detect name change? (same index elements but different name?)
            foreach (var indexNew in tableNew.Properties.OfType<TIndex>())
            {
                var indexOriginal = tableOriginal.Properties.OfType<TIndex>().FirstOrDefault(indexOriginal => indexOriginal.Name == indexNew.Name);
                if (indexOriginal == null)
                {
                    changes.Add(CreateNew(indexNew));
                }
                else
                {
                    var indexChanged = false;
                    var indexChange = CreateChange(indexOriginal, indexNew);

                    // compare referred tables
                    // compare columns
                    if (indexOriginal.SqlTable.SchemaAndTableName != indexNew.SqlTable.SchemaAndTableName)
                        indexChanged = true;

                    if (!CompareIndexColumns(indexOriginal, indexNew))
                    {
                        indexChanged = true;
                    }

                    if (indexChanged)
                        changes.Add(indexChange);
                }
            }

            return changes;
        }

        protected virtual bool CompareIndexColumns(TIndex indexOriginal, TIndex indexNew)
        {
            if (indexOriginal.SqlColumns.Count != indexNew.SqlColumns.Count)
                return false;

            for (var i = 0; i < indexOriginal.SqlColumns.Count; i++)
            {
                if (indexOriginal.SqlColumns[i].Order != indexNew.SqlColumns[i].Order)
                    return false;

                if (Comparer.ColumnChanged(indexOriginal.SqlColumns[i].SqlColumn, indexNew.SqlColumns[i].SqlColumn))
                    return false;
            }

            return true;
        }

        public abstract TMigration CreateDelete(TIndex originalIndex);

        public abstract TMigration CreateNew(TIndex originalIndex);

        public abstract TMigration CreateChange(TIndex originalIndex, TIndex newIndex);
    }
}
