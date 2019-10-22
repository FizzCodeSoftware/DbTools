using FizzCode.DbTools.DataDefinition;

namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    public class PatternMatchingTableCustomizerItem
    {
        public PatternMatchingTableCustomizerItem(SchemaAndTableName pattern, SchemaAndTableName patternExcept, bool shouldSkip, string category, string backGroundColor)
        {
            Pattern = pattern;
            ShouldSkipIfMatch = shouldSkip;
            CategoryIfMatch = category;
            BackGroundColorIfMatch = backGroundColor;
            PatternExcept = patternExcept;
        }

        public SchemaAndTableName Pattern { get; set; }

        public SchemaAndTableName PatternExcept { get; set; }

        public bool ShouldSkipIfMatch { get; set; }

        public string CategoryIfMatch { get; set; }

        public string BackGroundColorIfMatch { get; set; }

        public override string ToString()
        {
            return $"{Pattern}; {PatternExcept}; {(ShouldSkipIfMatch ? "1" : "0")}; {CategoryIfMatch}; {BackGroundColorIfMatch}";
        }
    }
}
