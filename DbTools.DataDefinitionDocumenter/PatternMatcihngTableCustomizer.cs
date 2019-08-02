namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Text.RegularExpressions;

    public class PatternMatchingTableCustomizer : ITableCustomizer
    {
        protected List<PatternMatchingTableCustomizerItem> _patterns = new List<PatternMatchingTableCustomizerItem>();

        public void AddPattern(string pattern, string patternExcept, bool shouldSkip, string category, string backGroundColor)
        {
            _patterns.Add(new PatternMatchingTableCustomizerItem(pattern, patternExcept, shouldSkip, category, backGroundColor));
        }

        public string BackGroundColor(string tableName)
        {
            var item = GetPatternMatching(tableName);
            return item?.BackGroundColorIfMatch;
        }

        public string Category(string tableName)
        {
            var item = GetPatternMatching(tableName);
            return item?.CategoryIfMatch;
        }

        public bool ShouldSkip(string tableName)
        {
            var item = GetPatternMatching(tableName);
            return item?.ShouldSkipIfMatch == true;
        }

        private PatternMatchingTableCustomizerItem GetPatternMatching(string tableName)
        {
            PatternMatchingTableCustomizerItem matchingItem = null;
            foreach (var item in _patterns)
            {
                if (IsRegex(item.Pattern))
                {
                    var regexPattern = Regex.Escape(item.Pattern).Replace(@"\*", ".*").Replace(@"\?", ".").Replace("#", @"\d");
                    if (Regex.Match(tableName, regexPattern).Success
                        && ShouldNotSkipPatternExcept(item, tableName))
                    {
                        if (matchingItem == null)
                            matchingItem = item;
                        else
                            throw new ApplicationException($"Multiple patterns are mathing for {tableName}.");
                    }
                }
                else if (item.Pattern == tableName
                    && ShouldNotSkipPatternExcept(item, tableName))
                {
                    matchingItem = item;
                    break;
                }
            }

            return matchingItem;
        }

        private bool ShouldNotSkipPatternExcept(PatternMatchingTableCustomizerItem item, string tableName)
        {
            if (IsRegex(item.PatternExcept))
            {
                var regexPatternExcept = Regex.Escape(item.PatternExcept).Replace(@"\*", ".*").Replace(@"\?", ".").Replace("#", @"\d");
                return !Regex.Match(tableName, regexPatternExcept).Success;
            }
            else
            {
                return item.PatternExcept != tableName;
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
