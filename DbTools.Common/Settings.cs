namespace FizzCode.DbTools.Common
{
    public class Settings
    {
        public SqlDialectSpecificSettings SqlDialectSpecificSettings { get; set; } = new SqlDialectSpecificSettings();
        public Options Options { get; set; } = new Options();
    }
}
