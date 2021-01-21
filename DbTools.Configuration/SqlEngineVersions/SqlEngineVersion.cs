namespace FizzCode.DbTools
{
    using System;
    using FizzCode.LightWeight.AdoNet;

    public abstract class SqlEngineVersion
    {
        public SqlEngine Engine { get; }
        public string UniqueName { get; }
        public string VersionString { get; }
        public string VersionNumber { get; }
        public string ProviderName { get; }

        protected SqlEngineVersion(SqlEngine engine, string uniqueName, string versionString, string versionNumber, string providerName)
        {
            Engine = engine;
            UniqueName = uniqueName;
            VersionString = versionString;
            VersionNumber = versionNumber;
            ProviderName = providerName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(UniqueName);
        }
        public override bool Equals(object obj)
        {
            return obj is SqlEngineVersion s && s.UniqueName == UniqueName;
        }

        public override string ToString()
        {
            return UniqueName;
        }
    }
}