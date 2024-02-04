namespace FizzCode.DbTools.Common;

public class Options
{
    public bool ShouldUseDefaultSchema { get; set; }
    public bool ShouldMigrateColumnChangesAllAtOnce { get; set; }
    public bool ShouldNotGuardKeywords { get; set; }
}
