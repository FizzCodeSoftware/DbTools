namespace FizzCode.DbTools.Interfaces
{
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition.Base;

    public interface ISqlGeneratorBase
    {
        SqlEngineVersion SqlVersion { get; }
        string GuardKeywords(string name);
        string GuardKeywordsImplementation(string name);
        Context Context { get; }
        string GetSimplifiedSchemaAndTableName(SchemaAndTableName schemaAndTableName);
        string GetSchema(SqlTable table);
        string GetSchema(string schema);
    }
}