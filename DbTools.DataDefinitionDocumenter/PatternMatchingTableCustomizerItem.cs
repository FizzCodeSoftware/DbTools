using System;
using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDefinitionDocumenter;
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

    public override bool Equals(object obj)
    {
        if (obj is not PatternMatchingTableCustomizerItem item)
            return false;

        return ((item.Pattern == null && Pattern == null) || item.Pattern.Equals(Pattern))
            && ((item.PatternExcept == null && PatternExcept == null) || item.PatternExcept.Equals(PatternExcept))
            && item.ShouldSkipIfMatch == ShouldSkipIfMatch
            && item.CategoryIfMatch == CategoryIfMatch
            && item.BackGroundColorIfMatch == BackGroundColorIfMatch;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hash = 17;
            hash = (hash * 23) + Pattern?.GetHashCode() ?? 0;
            hash = (hash * 23) + PatternExcept?.GetHashCode() ?? 0;
            hash = (hash * 23) + ShouldSkipIfMatch.GetHashCode();
            hash = (hash * 23) + CategoryIfMatch?.GetHashCode(StringComparison.InvariantCultureIgnoreCase) ?? 0;
            hash = (hash * 23) + BackGroundColorIfMatch?.GetHashCode(StringComparison.InvariantCultureIgnoreCase) ?? 0;
            return hash;
        }
    }
}
