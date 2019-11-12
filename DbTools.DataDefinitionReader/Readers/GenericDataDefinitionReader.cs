namespace FizzCode.DbTools.DataDefinitionReader
{
    using System.Collections.Generic;
    using FizzCode.DbTools.Common.Logger;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionExecuter;

    public abstract class GenericDataDefinitionReader : IDataDefinitionReader
    {
        protected GenericDataDefinitionReader(SqlExecuter sqlExecuter)
        {
            Executer = sqlExecuter;
        }

        protected SqlExecuter Executer { get; }

        protected Logger Logger => Executer.Generator.Context.Logger;

        public abstract List<SchemaAndTableName> GetSchemaAndTableNames();
        public abstract SqlTable GetTableDefinition(SchemaAndTableName schemaAndTableName, bool fullDefinition);

        public abstract DatabaseDefinition GetDatabaseDefinition();
    }
}
