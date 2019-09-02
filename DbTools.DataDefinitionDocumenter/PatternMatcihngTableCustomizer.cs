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

        private PatternMatchingTableCustomizerItem GetPatternMatching(SchemaAndTableName tableName)
        {
            PatternMatchingTableCustomizerItem matchingItem = null;
            foreach (var item in _patterns)
            {
                if (IsRegex(item.Pattern))
                {
                    var regexPattern = "^" + Regex.Escape(item.Pattern).Replace(@"\*", ".*").Replace(@"\?", ".").Replace("#", @"\d");
                    if ((Regex.Match(tableName.SchemaAndName, regexPattern).Success
                        || Regex.Match(tableName.TableName, regexPattern).Success)
                        && ShouldNotSkipPatternExcept(item, tableName))
                    {
                        if (matchingItem == null)
                            matchingItem = item;
                        else
                            throw new ApplicationException($"Multiple patterns are matching for {tableName.SchemaAndName}.");
                    }
                }
                else if ((string.Equals(item.Pattern, tableName.SchemaAndName, StringComparison.InvariantCultureIgnoreCase)
                    || string.Equals(item.Pattern, tableName.TableName, StringComparison.InvariantCultureIgnoreCase))
                    && ShouldNotSkipPatternExcept(item, tableName))
                {
                    matchingItem = item;
                    break;
                }
            }

            return matchingItem;
        }

        private bool ShouldNotSkipPatternExcept(PatternMatchingTableCustomizerItem item, SchemaAndTableName tableName)
        {
            if (IsRegex(item.PatternExcept))
            {
                var regexPatternExcept = Regex.Escape(item.PatternExcept).Replace(@"\*", ".*").Replace(@"\?", ".").Replace("#", @"\d");
                return !Regex.Match(tableName.SchemaAndName, regexPatternExcept).Success
                    && !Regex.Match(tableName.TableName, regexPatternExcept).Success;
            }
            else
            {
                return !string.Equals(item.PatternExcept, tableName.SchemaAndName, StringComparison.InvariantCultureIgnoreCase)
                    && !string.Equals(item.PatternExcept, tableName.TableName, StringComparison.InvariantCultureIgnoreCase);
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
