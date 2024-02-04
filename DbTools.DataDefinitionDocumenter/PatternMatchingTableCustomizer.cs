using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDefinitionDocumenter;
public class PatternMatchingTableCustomizer : ITableCustomizer
{
    public List<PatternMatchingTableCustomizerItem> Patterns { get; } = new List<PatternMatchingTableCustomizerItem>();

    public void AddPattern(string patternSchema, string patternTableName, string patternExceptSchema, string patternExceptTableName, bool shouldSkip, string category, string backGroundColor)
    {
        Patterns.Add(new PatternMatchingTableCustomizerItem(new SchemaAndTableName(patternSchema, patternTableName), new SchemaAndTableName(patternExceptSchema, patternExceptTableName), shouldSkip, category, backGroundColor));
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

    public PatternMatchingTableCustomizerItem GetPatternMatching(SchemaAndTableName schemaAndTableName)
    {
        PatternMatchingTableCustomizerItem matchingItem = null;
        foreach (var item in Patterns)
        {
            var isPatternMatch = CheckMatch(schemaAndTableName, item.Pattern);
            var isPatternExceptMatch = CheckMatch(schemaAndTableName, item.PatternExcept);

            if (isPatternMatch && !isPatternExceptMatch)
            {
                if (IsRegex(item.Pattern.Schema) || IsRegex(item.Pattern.TableName))
                {
                    if (matchingItem != null)
                        throw new ApplicationException($"Multiple patterns are matching for {schemaAndTableName.SchemaAndName}.");

                    matchingItem = item;
                }
                else
                {
                    matchingItem = item;
                    break;
                }
            }
        }

        return matchingItem;
    }

    public PatternMatchingTableCustomizerItem GetPatternMatching(SchemaAndTableName schemaAndTableName, out bool isMatchWithException)
    {
        isMatchWithException = false;

        PatternMatchingTableCustomizerItem matchingItem = null;
        foreach (var item in Patterns)
        {
            var isPatternMatch = CheckMatch(schemaAndTableName, item.Pattern);
            var isPatternExceptMatch = CheckMatch(schemaAndTableName, item.PatternExcept);

            if (isPatternMatch && isPatternExceptMatch)
                isMatchWithException = true;

            if (isPatternMatch && !isPatternExceptMatch)
            {
                if (IsRegex(item.Pattern.Schema) || IsRegex(item.Pattern.TableName))
                {
                    if (matchingItem != null)
                        throw new ApplicationException($"Multiple patterns are matching for {schemaAndTableName.SchemaAndName}.");

                    matchingItem = item;
                }
                else
                {
                    matchingItem = item;
                    break;
                }
            }
        }

        return matchingItem;
    }

    private static bool CheckMatch(SchemaAndTableName schemaAndTableNameActual, SchemaAndTableName schemaAndTableNamePattern)
    {
        if (schemaAndTableNamePattern.TableName == null && schemaAndTableNamePattern.Schema == null)
            return false;

        var isTableNameMatch = schemaAndTableNamePattern.TableName == null
            || CheckMatchRegexOrString(schemaAndTableNameActual.TableName, schemaAndTableNamePattern.TableName);

        if (schemaAndTableNamePattern.Schema == null)
            return isTableNameMatch;

        var isSchemaMatch = CheckMatchRegexOrString(schemaAndTableNameActual.Schema, schemaAndTableNamePattern.Schema);

        return isTableNameMatch && isSchemaMatch;
    }

    private static bool CheckMatchRegexOrString(string actual, string regexOrString)
    {
        if (IsRegex(regexOrString))
        {
            var regexPattern = RegexFormFromWildCharForm(regexOrString);
            if (regexPattern != null)
                regexPattern = "^" + regexPattern;

            return Regex.Match(actual, regexPattern).Success;
        }

        return string.Equals(actual, regexOrString, StringComparison.InvariantCultureIgnoreCase);
    }

    private static string RegexFormFromWildCharForm(string schemaOrTableName)
    {
        if (schemaOrTableName == null)
            return null;

        return Regex.Escape(schemaOrTableName).Replace(@"\*", ".*", StringComparison.OrdinalIgnoreCase).Replace(@"\?", ".", StringComparison.OrdinalIgnoreCase).Replace("#", @"\d", StringComparison.OrdinalIgnoreCase) + "$";
    }

    private static bool IsRegex(string pattern)
    {
        if (pattern == null)
            return false;

        return pattern.Contains("*", StringComparison.OrdinalIgnoreCase) || pattern.Contains("?", StringComparison.OrdinalIgnoreCase) || pattern.Contains("#", StringComparison.OrdinalIgnoreCase);
    }
}
