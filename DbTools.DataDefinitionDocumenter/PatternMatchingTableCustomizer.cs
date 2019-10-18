namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using FizzCode.DbTools.DataDefinition;

    public class PatternMatchingTableCustomizer : ITableCustomizer
    {
        protected List<PatternMatchingTableCustomizerItem> Patterns { get; } = new List<PatternMatchingTableCustomizerItem>();

        public void AddPattern(string pattern, string patternExcept, bool shouldSkip, string category, string backGroundColor)
        {
            Patterns.Add(new PatternMatchingTableCustomizerItem(pattern, patternExcept, shouldSkip, category, backGroundColor));
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

        private PatternMatchingTableCustomizerItem GetPatternMatching(SchemaAndTableName schemaAndTableName)
        {
            PatternMatchingTableCustomizerItem matchingItem = null;
            foreach (var item in Patterns)
            {
                if (IsRegex(item.Pattern))
                {
                    var regexPattern = "^" + Regex.Escape(item.Pattern).Replace(@"\*", ".*", StringComparison.OrdinalIgnoreCase).Replace(@"\?", ".", StringComparison.OrdinalIgnoreCase).Replace("#", @"\d", StringComparison.OrdinalIgnoreCase);
                    if ((Regex.Match(schemaAndTableName.SchemaAndName, regexPattern).Success
                        || Regex.Match(schemaAndTableName.TableName, regexPattern).Success)
                        && ShouldNotSkipPatternExcept(item, schemaAndTableName))
                    {
                        if (matchingItem == null)
                            matchingItem = item;
                        else
                            throw new ApplicationException($"Multiple patterns are matching for {schemaAndTableName.SchemaAndName}.");
                    }
                }
                else if ((string.Equals(item.Pattern, schemaAndTableName.SchemaAndName, StringComparison.InvariantCultureIgnoreCase)
                    || string.Equals(item.Pattern, schemaAndTableName.TableName, StringComparison.InvariantCultureIgnoreCase))
                    && ShouldNotSkipPatternExcept(item, schemaAndTableName))
                {
                    matchingItem = item;
                    break;
                }
            }

            return matchingItem;
        }

        private static bool ShouldNotSkipPatternExcept(PatternMatchingTableCustomizerItem item, SchemaAndTableName schemaAndTableName)
        {
            if (IsRegex(item.PatternExcept))
            {
                var regexPatternExcept = Regex.Escape(item.PatternExcept).Replace(@"\*", ".*", StringComparison.OrdinalIgnoreCase).Replace(@"\?", ".", StringComparison.OrdinalIgnoreCase).Replace("#", @"\d", StringComparison.OrdinalIgnoreCase);
                return !Regex.Match(schemaAndTableName.SchemaAndName, regexPatternExcept).Success
                    && !Regex.Match(schemaAndTableName.TableName, regexPatternExcept).Success;
            }

            return !string.Equals(item.PatternExcept, schemaAndTableName.SchemaAndName, StringComparison.InvariantCultureIgnoreCase)
                && !string.Equals(item.PatternExcept, schemaAndTableName.TableName, StringComparison.InvariantCultureIgnoreCase);
        }

        private static bool IsRegex(string pattern)
        {
            if (pattern == null)
                return false;

            return pattern.Contains("*", StringComparison.OrdinalIgnoreCase) || pattern.Contains("?", StringComparison.OrdinalIgnoreCase) || pattern.Contains("#", StringComparison.OrdinalIgnoreCase);
        }
    }
}
