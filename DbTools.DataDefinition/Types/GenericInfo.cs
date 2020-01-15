namespace FizzCode.DbTools.DataDefinition
{
    using FizzCode.DbTools.Common;

    public class GenericInfo : DbInfo
    {
        public GenericInfo()
        {
            var sqlTypeInfos = new SqlTypeInfos
            {
                new SqlTypeInfo("CHAR", true, false, false),
                new SqlTypeInfo("NCHAR", true, false, false),
                new SqlTypeInfo("VARCHAR", true, false, false),
                new SqlTypeInfo("NVARCHAR", true, false, false),

                new SqlTypeInfo("DEFAULTPK"),

                new SqlTypeInfo("FLOAT_SMALL"),
                new SqlTypeInfo("FLOAT_LARGE"),

                new SqlTypeInfo("BIT"),
                new SqlTypeInfo("BYTE"),
                new SqlTypeInfo("INT16", true, true, false),
                new SqlTypeInfo("INT32", true, true, false),
                new SqlTypeInfo("INT64", true, true, false),

                new SqlTypeInfo("NUMBER", true, true, false),

                new SqlTypeInfo("DATE"),
                new SqlTypeInfo("TIME"),
                new SqlTypeInfo("DATETIME"),
            };

            TypesPerVersions.Add(new Common.Generic1(), sqlTypeInfos);
        }

        public static SqlTypeInfos Current
        {
            get
            {
                return TypesPerVersions[SqlEngines.GetLatestVersion(SqlDialectX.Generic)];
            }
        }
    }
}