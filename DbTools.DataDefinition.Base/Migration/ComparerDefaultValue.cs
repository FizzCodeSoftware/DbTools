namespace FizzCode.DbTools.DataDefinition.Base.Migration;
public class ComparerDefaultValue : ComparerSqlColumnPropertyBase<DefaultValue, DefaultValueMigration>
{
    public static List<DefaultValueMigration> CompareDefaultValue(SqlColumn columnOriginal, SqlColumn columnNew)
    {
        var comparer = new ComparerDefaultValue();
        var changes = comparer.CompareProperties(columnOriginal, columnNew);
        return changes;
    }

    public override DefaultValueMigration CreateChange(DefaultValue originalProperty, DefaultValue newProperty)
    {
        return new DefaultValueChange()
        {
            DefaultValue = originalProperty,
            NewDefaultValue = newProperty
        };
    }

    public override DefaultValueMigration CreateDelete(DefaultValue originalProperty)
    {
        return new DefaultValueDelete()
        {
            DefaultValue = originalProperty
        };
    }

    public override DefaultValueMigration CreateNew(DefaultValue originalProperty)
    {
        return new DefaultValueNew()
        {
            DefaultValue = originalProperty
        };
    }

    protected override bool ComparePropertiesInternal(DefaultValue propertyOriginal, DefaultValue propertyNew)
    {
        return propertyOriginal.Value == propertyNew.Value;
    }
}
