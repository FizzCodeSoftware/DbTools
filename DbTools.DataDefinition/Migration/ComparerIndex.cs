namespace FizzCode.DbTools.DataDefinition.Migration
{
    using System.Collections.Generic;

    public class ComparerIndex : ComparerIndexBase<Index, IndexMigration>
    {
        public static new List<IndexMigration> CompareIndexes(SqlTable tableOriginal, SqlTable tableNew)
        {
            var comparer = new ComparerIndex();
            var changes = ((ComparerIndexBase<Index, IndexMigration>)comparer).CompareIndexes(tableOriginal, tableNew);
            return changes;
        }

        protected override bool CompareIndexColumns(Index indexOriginal, Index indexNew)
        {
            if (!base.CompareIndexColumns(indexOriginal, indexNew))
                return false;

            if (indexOriginal.Includes.Count != indexNew.Includes.Count)
                return false;

            for (var i = 0; i < indexOriginal.Includes.Count; i++)
            {
                if (indexOriginal.Includes[i].Name != indexNew.Includes[i].Name)
                    return false;

                if (Comparer.ColumnChanged(indexOriginal.Includes[i], indexNew.Includes[i]))
                    return false;
            }

            return true;
        }

        public override IndexMigration CreateDelete(Index originalIndex)
        {
            return new IndexDelete()
            {
                Index = originalIndex
            };
        }

        public override IndexMigration CreateNew(Index originalIndex)
        {
            return new IndexNew()
            {
                Index = originalIndex
            };
        }

        public override IndexMigration CreateChange(Index originalIndex, Index newIndex)
        {
            return new IndexChange()
            {
                Index = originalIndex,
                NewIndex = newIndex
            };
        }
    }
}
