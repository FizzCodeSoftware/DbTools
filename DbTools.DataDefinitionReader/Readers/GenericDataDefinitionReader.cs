namespace FizzCode.DbTools.DataDefinitionReader
{
    using System.Collections.Generic;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionExecuter;

    public abstract class GenericDataDefinitionReader : IDataDefinitionReader
    {
        protected GenericDataDefinitionReader(SqlExecuter sqlExecuter)
        {
            _executer = sqlExecuter;
        }

        protected readonly SqlExecuter _executer;

        public abstract List<string> GetTableNames();
        public abstract SqlTable GetTableDefinition(string tableName, bool fullDefinition);

        public abstract DatabaseDefinition GetDatabaseDefinition();
    }
}
