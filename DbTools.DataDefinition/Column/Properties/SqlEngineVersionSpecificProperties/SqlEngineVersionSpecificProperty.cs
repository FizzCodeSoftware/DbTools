namespace FizzCode.DbTools.DataDefinition
{
    using FizzCode.DbTools.Configuration;

    public class SqlEngineVersionSpecificProperty
    {
        public SqlEngineVersion Version { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public string Key
        {
            get
            {
                return GetKey(this);
            }
        }

        public static string GetKey(SqlEngineVersionSpecificProperty proeprty)
        {
            return GetKey(proeprty.Version, proeprty.Name);
        }

        public static string GetKey(SqlEngineVersion version, string name)
        {
            return version.ToString() + "/" + name;
        }
    }
}
