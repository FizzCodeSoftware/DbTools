namespace FizzCode.DbTools.DataDefinition.Migration
{
    using System.Collections.Generic;

    public class ComparerPrimaryKey : ComparerIndexBase<PrimaryKey, PrimaryKeyMigration>
    {
        public static List<PrimaryKeyMigration> ComparePrimaryKeys(SqlTable tableOriginal, SqlTable tableNew)
        {
            var comparer = new ComparerPrimaryKey();
            var changes = comparer.CompareIndexes(tableOriginal, tableNew);
            return changes;
        }

        public override PrimaryKeyMigration CreateChange(PrimaryKey originalIndex, PrimaryKey newIndex)
        {
            return new PrimaryKeyChange()
            {
                PrimaryKey = originalIndex,
                NewPrimaryKey = newIndex
            };
        }

        public override PrimaryKeyMigration CreateDelete(PrimaryKey originalIndex)
        {
            return new PrimaryKeyDelete()
            {
                PrimaryKey = originalIndex
            };
        }

        public override PrimaryKeyMigration CreateNew(PrimaryKey originalIndex)
        {
            return new PrimaryKeyNew()
            {
                PrimaryKey = originalIndex
            };
        }
    }
}
