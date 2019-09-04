namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using FizzCode.DbTools.DataDefinition;

    public class PatternMatchingTableCustomizer : ITableCustomizer
    {
        protected List<PatternMatchingTableCustomizerItem> _patterns = new List<PatternMatchingTableCustomizerItem>();

        public void AddPattern(string pattern, string patternExcept, bool shouldSkip, string category, string backGroundColor)
        {
            _patterns.Add(new PatternMatchingTableCustomizerItem(pattern, patternExcept, shouldSkip, category, backGroundColor));
        }

        public string BackGroundColor(SchemaAndTableName schemaAndTableName)
        {
            var item = GetPatternMatching(schemaAndTableName);
            return item?.BackGroundColorIfMatch;
        }

        public string Category(SchemaAndTableName schemaAndTableName)
        {
            var item = GetPatternMatching(schemaAndTableName);
            return item?.CategoryIfMatch;
        }

        public bool ShouldSkip(SchemaAndTableName schemaAndTableName)
        {
            var item = GetPatternMatching(schemaAndTableName);
            return item?.ShouldSkipIfMatch == true;
        }

        private PatternMatchingTableCustomizerItem GetPatternMatching(SchemaAndTableName schemaAndTableName)
        {
            PatternMatchingTableCustomizerItem matchingItem = null;
            foreach (var item in _patterns)
            {
                if (IsRegex(item.Pattern))
                {
                    var regexPattern = "^" + Regex.Escape(item.Pattern).Replace(@"\*", ".*").Replace(@"\?", ".").Replace("#", @"\d");
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

        private bool ShouldNotSkipPatternExcept(PatternMatchingTableCustomizerItem item, SchemaAndTableName schemaAndTableName)
        {
            if (IsRegex(item.PatternExcept))
            {
                var regexPatternExcept = Regex.Escape(item.PatternExcept).Replace(@"\*", ".*").Replace(@"\?", ".").Replace("#", @"\d");
                return !Regex.Match(schemaAndTableName.SchemaAndName, regexPatternExcept).Success
                    && !Regex.Match(schemaAndTableName.TableName, regexPatternExcept).Success;
            }
            else
            {
                return !string.Equals(item.PatternExcept, schemaAndTableName.SchemaAndName, StringComparison.InvariantCultureIgnoreCase)
                    && !string.Equals(item.PatternExcept, schemaAndTableName.TableName, StringComparison.InvariantCultureIgnoreCase);
            }
        }

        private bool IsRegex(string pattern)
        {
            if (pattern == null)
                return false;

            return pattern.Contains("*") || pattern.Contains("?") || pattern.Contains("#");
        }
    }
}
