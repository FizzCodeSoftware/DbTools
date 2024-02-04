namespace FizzCode.DbTools.DataDefinition.Base.Migration;
public abstract class ComparerSqlColumnPropertyBase<TProperty, TMigration>
    where TProperty : SqlColumnProperty
    where TMigration : SqlColumnPropertyMigration
{
    public List<TMigration> CompareProperties(SqlColumn columnOriginal, SqlColumn columnNew)
    {
        var changes = new List<TMigration>();

        foreach (var propertyOriginal in columnOriginal.Properties.OfType<TProperty>())
        {
            if (!columnNew.Properties.OfType<TProperty>().Any())
            {
                changes.Add(CreateDelete(propertyOriginal));
            }
        }

        foreach (var propertyNew in columnNew.Properties.OfType<TProperty>())
        {
            var propertyOriginal = columnOriginal.Properties.OfType<TProperty>().FirstOrDefault();
            if (propertyOriginal == null)
            {
                changes.Add(CreateNew(propertyNew));
            }
            else
            {
                var indexChanged = false;
                var indexChange = CreateChange(propertyOriginal, propertyNew);

                if (!ComparePropertiesInternal(propertyOriginal, propertyNew))
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

    protected abstract bool ComparePropertiesInternal(TProperty propertyOriginal, TProperty propertyNew);

    public abstract TMigration CreateDelete(TProperty originalProperty);

    public abstract TMigration CreateNew(TProperty originalProperty);

    public abstract TMigration CreateChange(TProperty originalProperty, TProperty newProperty);
}
