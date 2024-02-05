using FizzCode.DbTools.DataDefinition;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.QueryBuilder.Interfaces;
using System.Linq;
using System.Reflection;

namespace FizzCode.DbTools.DataDeclaration;
public static class DeclaredStoredProcedures
{
    public static void AddDeclaredStoredProcedures(DatabaseDeclaration dd)
    {
        var properties = dd.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(pi => typeof(StoredProcedure).IsAssignableFrom(pi.PropertyType));

        foreach (var property in properties)
        {
            var sp = property.GetValueSafe<StoredProcedure>(dd);

            if (sp is IStoredProcedureFromQuery spq)
            {
                var versions = dd.GetVersions();
                foreach (var version in versions)
                {
                    dd.QueryBuilderConnectors[version].ProcessStoredProcedureFromQuery(spq);
                }
            }

            if (sp.SchemaAndSpName is null)
            {
                var schemaAndTableName = new SchemaAndTableName(property.Name);
                if (string.IsNullOrEmpty(schemaAndTableName.Schema) && !string.IsNullOrEmpty(dd.DefaultSchema))
                    schemaAndTableName.Schema = dd.DefaultSchema;

                sp.SchemaAndSpName = schemaAndTableName;

                ReplaceStoredProcedureBodyIfThereIsOnlyGeneric1(dd, sp);
            }

            sp.DatabaseDefinition = dd;
            dd.StoredProcedures.Add(sp);
        }
    }

    private static void ReplaceStoredProcedureBodyIfThereIsOnlyGeneric1(DatabaseDeclaration dd, StoredProcedure sp)
    {
        if (sp.StoredProcedureBodies.Keys.Count == 1
            && sp.StoredProcedureBodies.Keys.First() == GenericVersion.Generic1)
        {
            sp.StoredProcedureBodies.Add(dd.MainVersion, sp.StoredProcedureBodies.Values.First());
        }
    }
}
