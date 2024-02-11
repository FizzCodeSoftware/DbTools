using FizzCode.DbTools.Common;
using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.Interfaces;
public interface ISqlGeneratorBase
{
    SqlEngineVersion SqlVersion { get; }
    string GuardKeywords(string name);
    string GuardKeywordsImplementation(string name);
    Context Context { get; }
    string GetSimplifiedSchemaAndTableName(SchemaAndTableName schemaAndTableName);
    string? GetSchema(SqlTable table);

}