namespace FizzCode.DbTools.SqlGenerator.Base
{
    using FizzCode.DbTools;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition.Base;
    using FizzCode.DbTools.Interfaces;

    public abstract class AbstractSqlGeneratorBase : ISqlGeneratorBase
    {
        public Context Context { get; }

        public abstract SqlEngineVersion SqlVersion { get; }

        protected AbstractSqlGeneratorBase(Context context)
        {
            Context = context;
        }

        public abstract string GuardKeywordsImplementation(string name);
        public string GuardKeywords(string name)
        {
            if (Context.Settings.Options.ShouldNotGuardKeywords)
                return name;

            return GuardKeywordsImplementation(name);
        }

        public string GetSimplifiedSchemaAndTableName(SchemaAndTableName schemaAndTableName)
        {
            var schema = GetSchema(schemaAndTableName.Schema);
            var tableName = schemaAndTableName.TableName;

            var defaultSchema = Context.Settings.SqlVersionSpecificSettings.GetAs<string>("DefaultSchema", null);

            if (!string.IsNullOrEmpty(defaultSchema) && Context.Settings.Options.ShouldUseDefaultSchema && string.IsNullOrEmpty(schema))
            {
                return GuardKeywords(defaultSchema) + "." + GuardKeywords(tableName);
            }

            if (!string.IsNullOrEmpty(schema) && (string.IsNullOrEmpty(defaultSchema) || !string.Equals(schema, defaultSchema, StringComparison.InvariantCultureIgnoreCase)))
            {
                return GuardKeywords(schema) + "." + GuardKeywords(tableName);
            }

            return GuardKeywords(tableName);
        }

        public string GetSchema(SqlTable table)
        {
            return GetSchema(table.SchemaAndTableName.Schema);
        }

        public virtual string GetSchema(string schema)
        {
            return schema;
        }
    }
}