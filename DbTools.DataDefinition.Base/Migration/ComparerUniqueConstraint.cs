namespace FizzCode.DbTools.DataDefinition.Base.Migration;
public class ComparerUniqueConstraint : ComparerIndexBase<UniqueConstraint, UniqueConstraintMigration>
{
    public static List<UniqueConstraintMigration> CompareUniqueConstraints(SqlTable tableOriginal, SqlTable tableNew)
    {
        var comparer = new ComparerUniqueConstraint();
        var changes = comparer.CompareIndexes(tableOriginal, tableNew);
        return changes;
    }

    public override UniqueConstraintMigration CreateDelete(UniqueConstraint originalIndex)
    {
        return new UniqueConstraintDelete()
        {
            UniqueConstraint = originalIndex
        };
    }

    public override UniqueConstraintMigration CreateNew(UniqueConstraint originalIndex)
    {
        return new UniqueConstraintNew()
        {
            UniqueConstraint = originalIndex
        };
    }

    public override UniqueConstraintMigration CreateChange(UniqueConstraint originalIndex, UniqueConstraint newIndex)
    {
        return new UniqueConstraintChange()
        {
            UniqueConstraint = originalIndex,
            NewUniqueConstraint = newIndex
        };
    }
}
