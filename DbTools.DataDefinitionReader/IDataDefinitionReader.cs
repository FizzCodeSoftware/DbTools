namespace FizzCode.DbTools.DataDefinitionReader
{
    using System.Collections.Generic;
    using FizzCode.DbTools.DataDefinition;

    public interface IDataDefinitionReader
    {
        DatabaseDefinition GetDatabaseDefinition();
        List<string> GetTableNames();
        SqlTable GetTableDefinition(string tableName, bool fullDefinition = true);
    }
}
