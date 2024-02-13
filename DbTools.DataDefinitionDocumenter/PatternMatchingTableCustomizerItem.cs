using System;

namespace FizzCode.DbTools.DataDefinitionDocumenter;
public class PatternMatchingTableCustomizerItem(SchemaAndTableNamePattern? pattern, SchemaAndTableNamePattern? patternExcept, bool shouldSkip, string? category, string? backGroundColor)
{
    public SchemaAndTableNamePattern? Pattern { get; set; } = pattern;

    public SchemaAndTableNamePattern? PatternExcept { get; set; } = patternExcept;

    public bool ShouldSkipIfMatch { get; set; } = shouldSkip;

    public string? CategoryIfMatch { get; set; } = category;

    public string? BackGroundColorIfMatch { get; set; } = backGroundColor;

    public override string ToString()
    {
        return $"{Pattern}; {PatternExcept}; {(ShouldSkipIfMatch ? "1" : "0")}; {CategoryIfMatch}; {BackGroundColorIfMatch}";
    }

    public override bool Equals(object? obj)
    {
        if (obj is not PatternMatchingTableCustomizerItem item)
            return false;

        return ((item.Pattern == null && Pattern is null) || (item.Pattern is not null && item.Pattern.Equals(Pattern))
            && ((item.PatternExcept == null && PatternExcept is null) || (item.PatternExcept is not null && item.PatternExcept.Equals(PatternExcept))))
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
