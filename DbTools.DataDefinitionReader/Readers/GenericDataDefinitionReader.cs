namespace FizzCode.DbTools.DataDefinitionReader
{
    using System.Collections.Generic;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Common.Logger;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionExecuter;

    public abstract class GenericDataDefinitionReader : IDataDefinitionReader
    {
        protected GenericDataDefinitionReader(SqlExecuter sqlExecuter)
        {
            Executer = sqlExecuter;
        }

        protected abstract SqlDialectX SqlDialect { get; }

        protected SqlExecuter Executer { get; }

        protected Logger Logger => Executer.Generator.Context.Logger;

        public abstract List<SchemaAndTableName> GetSchemaAndTableNames();
        public abstract SqlTable GetTableDefinition(SchemaAndTableName schemaAndTableName, bool fullDefinition);

        public abstract DatabaseDefinition GetDatabaseDefinition();

        protected void Log(LogSeverity severity, string text, params object[] args)
        {
            var module = "Reader/" + Executer. SqlDialect.ToString();
            Logger.Log(severity, text, module, args);
        }

        public static SchemaAndTableName GetSchemaAndTableNameAsToStore(SchemaAndTableName original, Context context)
        {
            var defaultSchema = context.Settings.SqlDialectSpecificSettings.GetAs<string>("DefaultSchema");

            if (context.Settings.Options.ShouldUseDefaultSchema && original.Schema == defaultSchema)
                return new SchemaAndTableName(null, original.TableName);

            return new SchemaAndTableName(original.Schema, original.TableName);
        }
    }
}
