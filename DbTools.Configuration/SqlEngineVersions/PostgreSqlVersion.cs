namespace FizzCode.DbTools.Configuration
{
    public class PostgreSqlVersion : SqlEngineVersion
    {
        internal PostgreSqlVersion(string uniqueName, string versionString, string versionNumber)
            : base(SqlEngine.PostgreSql, uniqueName, versionString, versionNumber, "Npgsql")
        {
        }

        public static PostgreSqlVersion Generic { get; } = new PostgreSqlVersion(nameof(Generic), null, null);
    }
}