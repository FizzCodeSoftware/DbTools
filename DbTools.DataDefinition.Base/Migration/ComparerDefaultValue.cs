using FizzCode.DbTools.Common;

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

    public override bool CompareProperties(DefaultValue propertyOriginal, DefaultValue propertyNew)
    {
        Throw.InvalidOperationExceptionIfNull(propertyOriginal.SqlColumn.SqlTableOrView);
        Throw.InvalidOperationExceptionIfNull(propertyOriginal.SqlColumn.SqlTableOrView.DatabaseDefinition);
        var version = propertyOriginal.SqlColumn.SqlTableOrView.DatabaseDefinition.MainVersion;

        // TODO engine specific comparison move to Implementation
        if (propertyOriginal.Value == propertyNew.Value)
            return true;

        if (propertyNew.Value.StartsWith("((") && propertyNew.Value.EndsWith("))"))
            return propertyOriginal.Value == propertyNew.Value.Substring(2, propertyNew.Value.Length - 4);

        if (propertyNew.Value.StartsWith('(') && propertyNew.Value.EndsWith(')'))
            return propertyOriginal.Value == propertyNew.Value.Substring(1, propertyNew.Value.Length - 2);

        if (propertyNew.Value.StartsWith('(') && propertyNew.Value.EndsWith(") ")) // ora spec
            return propertyOriginal.Value == propertyNew.Value.Substring(1, propertyNew.Value.Length - 3);

        return propertyOriginal.Value == propertyNew.Value;
    }
}
