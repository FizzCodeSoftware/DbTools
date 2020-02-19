namespace FizzCode.DbTools.Configuration
{
    public class MySqlVersion : SqlEngineVersion
    {
        internal MySqlVersion(string uniqueName, string versionString, string versionNumber)
            : base(SqlEngine.PostgreSql, uniqueName, versionString, versionNumber, "MySql.Data.MySqlClient")
        {
        }

        public static MySqlVersion Generic { get; } = new MySqlVersion(nameof(Generic), null, null);
    }
}