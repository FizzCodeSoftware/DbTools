using System.Collections.Generic;
using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDefinitionDocumenter;
public class PatternMatchingTableCustomizerWithTablesAndItems : ITableCustomizer
{
    public PatternMatchingTableCustomizerWithTablesAndItems(PatternMatchingTableCustomizer patternMatchingTableCustomizer)
    {
        PatternMatchingTableCustomizer = patternMatchingTableCustomizer;
    }

    public Dictionary<SchemaAndTableName, List<PatternMatchingTableCustomizerItem>> TableMatches { get; } = [];

    public Dictionary<PatternMatchingTableCustomizerItem, List<SchemaAndTableName>> MatchTables { get; } = [];

    public Dictionary<PatternMatchingTableCustomizerItem, List<SchemaAndTableName>> MatchTablesWithException { get; } = [];

    public PatternMatchingTableCustomizer PatternMatchingTableCustomizer { get; }

    protected PatternMatchingTableCustomizerItem GetPatternMatching(SchemaAndTableName schemaAndTableName)
    {
        var item = PatternMatchingTableCustomizer.GetPatternMatching(schemaAndTableName, out var isMatchWithException);

        if (item != null)
        {
            if (!TableMatches.ContainsKey(schemaAndTableName))
                TableMatches.Add(schemaAndTableName, []);

            if (!TableMatches[schemaAndTableName].Contains(item))
                TableMatches[schemaAndTableName].Add(item);

            if (!MatchTables.ContainsKey(item))
                MatchTables.Add(item, []);

            if (!MatchTables[item].Contains(schemaAndTableName))
                MatchTables[item].Add(schemaAndTableName);
        }

        if (isMatchWithException)
        {
            if (!MatchTablesWithException.ContainsKey(item))
                MatchTablesWithException.Add(item, []);

            if (!MatchTablesWithException[item].Contains(schemaAndTableName))
                MatchTablesWithException[item].Add(schemaAndTableName);
        }

        return item;
    }

    public string BackGroundColor(SchemaAndTableName tableName)
    {
        var item = GetPatternMatching(tableName);
        return item?.BackGroundColorIfMatch;
    }

    public string Category(SchemaAndTableName tableName)
    {
        var item = GetPatternMatching(tableName);
        return item?.CategoryIfMatch;
    }

    public bool ShouldSkip(SchemaAndTableName tableName)
    {
        var item = GetPatternMatching(tableName);
        return item?.ShouldSkipIfMatch == true;
    }
}
