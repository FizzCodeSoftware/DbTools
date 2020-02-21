namespace FizzCode.DbTools.DataDefinitionReader
{
    using System.Collections.Generic;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Common.Logger;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.SqlExecuter;

    public abstract class GenericDataDefinitionReader : IDataDefinitionReader
    {
        protected GenericDataDefinitionReader(SqlStatementExecuter executer, List<string> schemaNames = null)
        {
            Executer = executer;
            if(schemaNames != null)
                SchemaNames = schemaNames;
        }

        protected List<string> SchemaNames { get; } = new List<string>();

        protected SqlStatementExecuter Executer { get; set; }

        protected Logger Logger => Executer.Generator.Context.Logger;

        public abstract List<SchemaAndTableName> GetSchemaAndTableNames();
        public abstract SqlTable GetTableDefinition(SchemaAndTableName schemaAndTableName, bool fullDefinition);

        public abstract DatabaseDefinition GetDatabaseDefinition();

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
    }
}
