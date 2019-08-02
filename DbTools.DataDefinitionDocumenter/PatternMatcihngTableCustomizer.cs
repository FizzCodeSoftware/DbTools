namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public class PatternMatchingTableCustomizer : ITableCustomizer
    {
        protected List<PatternMatchingTableCustomizerItem> _patterns = new List<PatternMatchingTableCustomizerItem>();

        public void AddPattern(string pattern, bool shouldSkip, string category, string backGroundColor)
        {
            _patterns.Add(new PatternMatchingTableCustomizerItem(pattern, shouldSkip, category, backGroundColor));
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

        public PatternMatchingTableCustomizerItem GetPatternMatching(string tableName)
        {
            PatternMatchingTableCustomizerItem matchingItem = null;
            foreach (var item in _patterns)
            {
                var regexPattern= Regex.Escape(item.Pattern).Replace(@"\*", ".*").Replace(@"\?", ".").Replace("#", @"\d");
                if (Regex.Match(tableName, regexPattern).Success)
                {
                    if (matchingItem == null)
                        matchingItem = item;
                    else
                        throw new ApplicationException($"Multiple patterns are mathing for {tableName}.");
                }
            }

            return matchingItem;
        }
    }

    public class PatternMatchingTableCustomizerItem
    {
        public PatternMatchingTableCustomizerItem(string pattern, bool shouldSkip, string category, string backGroundColor)
        {
            Pattern = pattern;
            ShouldSkipIfMatch = shouldSkip;
            CategoryIfMatch = category;
            BackGroundColorIfMatch = backGroundColor;
        }

        public string Pattern { get; set; }

        public bool ShouldSkipIfMatch { get; set; }

        public string CategoryIfMatch { get; set; }

        public string BackGroundColorIfMatch { get; set; }
    }
}
