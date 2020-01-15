using System.Collections.Generic;
using FizzCode.DbTools.Common;

namespace FizzCode.DbTools.DataDefinition
{

    public abstract class DbInfo
    {
        protected static Dictionary<SqlVersion, SqlTypeInfos> TypesPerVersions { get; } = new Dictionary<SqlVersion, SqlTypeInfos>();

        public static SqlTypeInfos Get(SqlVersion version)
        {
            return TypesPerVersions[version];
        }
    }
}