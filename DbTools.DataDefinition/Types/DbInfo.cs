namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;
    using FizzCode.DbTools.Configuration;

    public abstract class DbInfo
    {
        protected static Dictionary<SqlVersion, SqlTypeInfos> TypesPerVersions { get; } = new Dictionary<SqlVersion, SqlTypeInfos>();

        public static SqlTypeInfos Get(SqlVersion version)
        {
            return TypesPerVersions[version];
        }
    }
}