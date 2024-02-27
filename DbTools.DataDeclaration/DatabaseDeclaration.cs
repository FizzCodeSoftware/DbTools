using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FizzCode.DbTools.Common;
using FizzCode.DbTools.DataDefinition;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.Factory.Interfaces;
using FizzCode.DbTools.QueryBuilder.Interfaces;

namespace FizzCode.DbTools.DataDeclaration;
public class DatabaseDeclaration : DatabaseDefinition, IDatabaseDeclaration
{
    public NamingStrategies NamingStrategies { get; }
    public string? DefaultSchema { get; }
    public QueryBuilderConnectors QueryBuilderConnectors { get; }

    protected DatabaseDeclaration(IFactoryContainer factoryContainer, SqlEngineVersion mainVersion, SqlEngineVersion[]? secondaryVersions = null, string? defaultSchema = null, NamingStrategies? namingStrategies = null)
        : base(factoryContainer, mainVersion, secondaryVersions)
    {
        DefaultSchema = defaultSchema;
        NamingStrategies = namingStrategies ?? new NamingStrategies();

        var queryBuilderFactory = FactoryContainer.Get<IQueryBuilderFactory>();
        QueryBuilderConnectors = new QueryBuilderConnectors(queryBuilderFactory);

        AddDeclaredTables();
        DeclaredStoredProcedures.AddDeclaredStoredProcedures(this);
        AddDeclaredViews();
        CreateRegisteredForeignKeys();
        AddAutoNaming(GetTables());
        CircularFKDetector.DectectCircularFKs(GetTables());
    }

    private static IEnumerable<T> GetProperties<T>(SqlTable sqlTable)
    {
        return sqlTable.Properties.OfType<T>().ToList();
    }

    public void CreateRegisteredForeignKeys()
    {
        foreach (var sqlTable in Tables)
        {
            CreateRegisteredForeignKeys(sqlTable);
        }
    }

    public void CreateRegisteredForeignKeys(SqlTable sqlTable)
    {
        foreach (var fkRegistration in GetProperties<ForeignKeyRegistrationToTableWithUniqueKeySingleColumn>(sqlTable))
        {
            if (DefaultSchema is not null && fkRegistration.ReferredTableName is not null && string.IsNullOrEmpty(fkRegistration.ReferredTableName.Schema))
                fkRegistration.ReferredTableName.Schema = DefaultSchema;

            RegisteredForeignKeysCreator.UniqueKeySingleColumn(this, sqlTable, fkRegistration);
        }

        foreach (var fkRegistration in GetProperties<ForeignKeyRegistrationToTableWithUniqueKey>(sqlTable))
        {
            if (DefaultSchema is not null && fkRegistration.ReferredTableName is not null && string.IsNullOrEmpty(fkRegistration.ReferredTableName.Schema))
                fkRegistration.ReferredTableName.Schema = DefaultSchema;

            RegisteredForeignKeysCreator.UniqueKey(this, sqlTable, fkRegistration, NamingStrategies.ForeignKey);
        }

        foreach (var fkRegistration in GetProperties<ForeignKeyRegistrationToTableWithUniqueKeyExistingColumn>(sqlTable))
        {
            if (DefaultSchema is not null && fkRegistration.ReferredTableName is not null && string.IsNullOrEmpty(fkRegistration.ReferredTableName.Schema))
                fkRegistration.ReferredTableName.Schema = DefaultSchema;

            RegisteredForeignKeysCreator.PrimaryKeyExistingColumn(this, sqlTable, fkRegistration);
        }

        foreach (var fkRegistration in GetProperties<ForeignKeyRegistrationToReferredTableExistingColumns>(sqlTable))
        {
            if (DefaultSchema is not null && fkRegistration.ReferredTableName is not null && string.IsNullOrEmpty(fkRegistration.ReferredTableName.Schema))
                fkRegistration.ReferredTableName.Schema = DefaultSchema;

            RegisteredForeignKeysCreator.ReferredTableExistingColumns(this, sqlTable, fkRegistration);
        }

        foreach (var fkRegistration in GetProperties<ForeignKeyRegistrationToReferredTable>(sqlTable))
        {
            if (DefaultSchema is not null && fkRegistration.ReferredTableName is not null && string.IsNullOrEmpty(fkRegistration.ReferredTableName.Schema))
                fkRegistration.ReferredTableName.Schema = DefaultSchema;

            RegisteredForeignKeysCreator.ReferredTable(this, sqlTable, fkRegistration);
        }
    }

    public void AddAutoNaming(List<SqlTable> tables)
    {
        foreach (var sqlTable in tables)
        {
            foreach (var pk in sqlTable.Properties.OfType<PrimaryKey>().Where(pk => string.IsNullOrEmpty(pk.Name)))
            {
                NamingStrategies.PrimaryKey.SetPrimaryKeyName(pk);
            }

            foreach (var index in sqlTable.Properties.OfType<Index>().Where(idx => string.IsNullOrEmpty(idx.Name)))
            {
                NamingStrategies.Index.SetIndexName(index);
            }

            foreach (var uniqueConstraint in sqlTable.Properties.OfType<UniqueConstraint>().Where(uc => string.IsNullOrEmpty(uc.Name)))
            {
                NamingStrategies.UniqueConstraint.SetUniqueConstraintName(uniqueConstraint);
            }

            foreach (var fk in sqlTable.Properties.OfType<ForeignKey>().Where(fk => string.IsNullOrEmpty(fk.Name)))
            {
                NamingStrategies.ForeignKey.SetFKName(fk);
            }

            foreach (var column in sqlTable.Columns)
            {
                var defaultValue = column.Properties.OfType<DefaultValue>().Where(dv => string.IsNullOrEmpty(dv.Name)).FirstOrDefault();
                if (defaultValue is not null)
                    NamingStrategies.DefaultValue.SetDefaultValueName(defaultValue);
            }
        }
    }

    private void AddDeclaredTables()
    {
        var properties = GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(pi => typeof(SqlTable).IsAssignableFrom(pi.PropertyType));

        foreach (var property in properties)
        {
            var table = property.GetValueSafe<SqlTable>(this);

            if (table.SchemaAndTableName is null)
            {
                var schemaAndTableName = new SchemaAndTableName(property.Name);
                if (string.IsNullOrEmpty(schemaAndTableName.Schema) && !string.IsNullOrEmpty(DefaultSchema))
                    schemaAndTableName.Schema = DefaultSchema;

                table.SchemaAndTableName = schemaAndTableName;
            }

            table.DatabaseDefinition = this;
            SetupDeclaredTable(table);
            AddTable(table);
        }

        var fields = GetType()
            .GetFields()
            .Where(fi => fi.IsPublic && fi.FieldType == typeof(SqlTable))
            .ToList();

        if (fields.Count > 0)
        {
            throw new System.InvalidOperationException(nameof(DatabaseDeclaration) + " is only compatible with tabled defined in public properties. Please review the following fields: " + string.Join(", ", fields.Select(fi => fi.Name)));
        }
    }

    private static void SetupDeclaredTable(SqlTable table)
    {
        AddDeclaredColumns(table);
        AddDeclaredForeignKeys(table);
        UpdateDeclaredIndexes(table);
        UpdateDeclaredCustomProperties(table);
    }

    private static void AddDeclaredColumns(SqlTable table)
    {
        var properties = table.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(pi => typeof(SqlColumn).IsAssignableFrom(pi.PropertyType) && pi.GetIndexParameters().Length == 0);

        foreach (var property in properties)
        {
            var column = property.GetValueSafe<SqlColumn>(table);
            column.Name = property.Name;

            var tablePlaceHolderProperties = column.Table.Properties;

            foreach (var tablePlaceHolderProperty in tablePlaceHolderProperties)
            {
                tablePlaceHolderProperty.SqlTableOrView = table;
                AddTablePlaceHolderProperty(table, column, tablePlaceHolderProperty);
            }

            column.Table = table;

            table.Columns.Add(column);
        }
    }

    private static void AddTablePlaceHolderProperty(SqlTable table, SqlColumn column, SqlTableOrViewPropertyBase<SqlTable> tablePlaceHolderProperty)
    {
        if (tablePlaceHolderProperty is PrimaryKey pk
            && table.HasProperty<PrimaryKey>())
        {
            table.SetPK(column, pk.SqlColumns[0].Order, pk.Name);
        }
        else
        {
            table.Properties.Add(tablePlaceHolderProperty);
        }
    }

    private void AddDeclaredViews()
    {
        var properties = GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(pi => typeof(SqlView).IsAssignableFrom(pi.PropertyType));

        foreach (var property in properties)
        {
            var view = property.GetValueSafe<SqlView>(this);

            if (view is IViewFromQuery vq)
            {
                var versions = this.GetVersions();
                foreach (var version in versions)
                {
                    QueryBuilderConnectors[version].ProcessViewFromQuery(vq);
                }
            }

            if (view.SchemaAndTableName is null)
            {
                var schemaAndTableName = new SchemaAndTableName(property.Name);
                if (string.IsNullOrEmpty(schemaAndTableName.Schema) && !string.IsNullOrEmpty(DefaultSchema))
                    schemaAndTableName.Schema = DefaultSchema;

                view.SchemaAndTableName = schemaAndTableName;
            }

            view.DatabaseDefinition = this;

            Views.Add(view);
        }
    }

    protected static SqlTable AddTable(System.Action<SqlTable> configurator)
    {
        var table = new SqlTable();
        configurator.Invoke(table);
        return table;
    }

    private static void UpdateDeclaredIndexes(SqlTable table)
    {
        var properties = table.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(pi =>
                (typeof(Index).IsAssignableFrom(pi.PropertyType)
                || typeof(UniqueConstraint).IsAssignableFrom(pi.PropertyType))
                && pi.GetIndexParameters().Length == 0);

        foreach (var property in properties)
        {
            var index = property.GetValueSafe<IndexBase<SqlTable>>(table);

            if (!property.Name.StartsWith('_'))
                index.Name = property.Name;

            index.SqlTableOrView = table;

            var registeredIdexes = index.SqlColumnRegistrations.ToList();

            foreach (var cr in registeredIdexes)
            {
                index.SqlColumnRegistrations.Remove(cr);
                index.SqlColumns.Add(new ColumnAndOrder(table.Columns[cr.ColumnName], cr.Order));
            }

            table.Properties.Add(index);
        }
    }

    private static void AddDeclaredForeignKeys(SqlTable table)
    {
        var properties = table.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(pi =>
                typeof(ForeignKey).IsAssignableFrom(pi.PropertyType)
                && pi.GetIndexParameters().Length == 0);

        foreach (var property in properties)
        {
            var fk = property.GetValueSafe<ForeignKey>(table);

            if (!property.Name.StartsWith('_'))
                fk.Name = property.Name;

            fk.SqlTableOrView = table;

            table.Properties.Add(fk);
        }
    }

    private static void UpdateDeclaredCustomProperties(SqlTable table)
    {
        var properties = table.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(pi =>
                typeof(SqlTableCustomProperty).IsAssignableFrom(pi.PropertyType)
                && pi.GetIndexParameters().Length == 0);

        foreach (var property in properties)
        {
            var customProperty = property.GetValueSafe<SqlTableCustomProperty>(table);
            customProperty.SqlTableOrView = table;
            table.Properties.Add(customProperty);
        }
    }

    private static void UpdateDeclaredCustomProperties(SqlView view)
    {
        var properties = view.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(pi =>
                typeof(SqlViewCustomProperty).IsAssignableFrom(pi.PropertyType)
                && pi.GetIndexParameters().Length == 0);

        foreach (var property in properties)
        {
            var customProperty = property.GetValueSafe<SqlViewCustomProperty>(view);
            customProperty.SqlTableOrView = view;
            view.Properties.Add(customProperty);
        }
    }
}
