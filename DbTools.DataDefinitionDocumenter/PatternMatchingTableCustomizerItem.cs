namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    public class PatternMatchingTableCustomizerItem
    {
        public PatternMatchingTableCustomizerItem(string pattern, string patternExcept, bool shouldSkip, string category, string backGroundColor)
        {
            Pattern = pattern;
            ShouldSkipIfMatch = shouldSkip;
            CategoryIfMatch = category;
            BackGroundColorIfMatch = backGroundColor;
            PatternExcept = patternExcept;
        }

        public string Pattern { get; set; }

        public string PatternExcept { get; set; }

        public bool ShouldSkipIfMatch { get; set; }

        public string CategoryIfMatch { get; set; }

        public string BackGroundColorIfMatch { get; set; }

        public override string ToString()
        {
            return $"{Pattern}; {PatternExcept}; {(ShouldSkipIfMatch ? "1" : "0")}; {CategoryIfMatch}; {BackGroundColorIfMatch}";
        }
    }
}
