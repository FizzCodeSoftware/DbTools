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
                Identity = originalProperty,
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
}
