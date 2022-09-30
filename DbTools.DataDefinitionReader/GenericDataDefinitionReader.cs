namespace FizzCode.DbTools.DataDefinitionReader
{
    using System.Collections.Generic;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Common.Logger;
    using FizzCode.DbTools.DataDefinition.Base;
    using FizzCode.DbTools.SqlExecuter;

    public abstract class GenericDataDefinitionReader : GenericDataDefinitionElementReader, IDataDefinitionReader
    {
        protected GenericDataDefinitionReader(SqlStatementExecuter executer, SchemaNamesToRead schemaNames)
            : base(executer, schemaNames)
        {
        }

        public abstract List<SchemaAndTableName> GetSchemaAndTableNames();
        public abstract SqlTable GetTableDefinition(SchemaAndTableName schemaAndTableName, bool fullDefinition);

        public abstract IDatabaseDefinition GetDatabaseDefinition();

        protected void Log(LogSeverity severity, string text, params object[] args)
        {
            var module = "Reader/" + Executer.Generator.Version.UniqueName;
            Logger.Log(severity, text, module, args);
        }

        public static SchemaAndTableName GetSchemaAndTableNameAsToStore(SchemaAndTableName original, Context context)
        {
            var defaultSchema = context.Settings.SqlVersionSpecificSettings.GetAs<string>("DefaultSchema");

            if (context.Settings.Options.ShouldUseDefaultSchema && original.Schema == defaultSchema)
                return new SchemaAndTableName(null, original.TableName);

            return new SchemaAndTableName(original.Schema, original.TableName);
        }

        public abstract List<SchemaAndTableName> GetViews();
        public abstract SqlView GetViewDefinition(SchemaAndTableName schemaAndTableName, bool fullDefinition);
    }
}
