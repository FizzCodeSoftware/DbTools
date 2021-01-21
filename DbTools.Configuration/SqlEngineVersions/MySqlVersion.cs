namespace FizzCode.DbTools
{
    using FizzCode.LightWeight.AdoNet;

    public class MySqlVersion : SqlEngineVersion
    {
        internal MySqlVersion(string uniqueName, string versionString, string versionNumber)
            : base(SqlEngine.MySql, uniqueName, versionString, versionNumber, "MySql.Data.MySqlClient")
        {
        }

        public static MySqlVersion Generic { get; } = new MySqlVersion(nameof(Generic), null, null);
    }
}