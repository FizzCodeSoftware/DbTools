namespace FizzCode.DbTools.DataDefinition.Migration
{
    using System.Collections.Generic;

    public class ComparerIdentity : ComparerSqlColumnPropertyBase<Identity, IdentityMigration>
    {
        public static List<IdentityMigration> CompareIdentity(SqlColumn columnOriginal, SqlColumn columnNew)
        {
            var comparer = new ComparerIdentity();
            var changes = comparer.CompareProperties(columnOriginal, columnNew);
            return changes;
        }

        public override IdentityMigration CreateChange(Identity originalProperty, Identity newProperty)
        {
            return new IdentityChange()
            {
                Identity  = originalProperty,
                NewIdentity = newProperty
            };
        }

        public override IdentityMigration CreateDelete(Identity originalProperty)
        {
            return new IdentityDelete()
            {
                Identity = originalProperty
            };
        }

        public override IdentityMigration CreateNew(Identity originalProperty)
        {
            return new IdentityNew()
            {
                Identity = originalProperty
            };
        }

        protected override bool ComparePropertiesInternal(Identity propertyOriginal, Identity propertyNew)
        {
            return propertyOriginal.Seed == propertyNew.Seed
                && propertyOriginal.Increment == propertyNew.Increment;
        }
    }

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
}
