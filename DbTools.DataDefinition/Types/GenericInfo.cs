namespace FizzCode.DbTools.DataDefinition
{
    using FizzCode.DbTools.Configuration;

    public class GenericInfo : DbInfo
    {
        static GenericInfo()
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

            TypesPerVersions.Add(new Configuration.Generic1(), sqlTypeInfos);
        }

        public static SqlTypeInfos Current
        {
            get
            {
                return TypesPerVersions[SqlEngines.GetLatestVersion<IGenericDialect>()];
            }
        }
    }
}