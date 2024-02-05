namespace FizzCode.DbTools.Common;

public class Settings
{
    public SqlVersionSpecificSettings SqlVersionSpecificSettings { get; set; } = [];
    public Options Options { get; set; } = new Options();
}
