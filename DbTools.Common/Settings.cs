namespace FizzCode.DbTools.Common
{
    public class Settings
    {
        public SqlVersionSpecificSettings SqlVersionSpecificSettings { get; set; } = new SqlVersionSpecificSettings();
        public Options Options { get; set; } = new Options();
    }
}
