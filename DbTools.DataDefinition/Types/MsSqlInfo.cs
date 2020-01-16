namespace FizzCode.DbTools.DataDefinition
{
    using FizzCode.DbTools.Configuration;

    public class MsSqlInfo : DbInfo
    {
        static MsSqlInfo()
        {
            var sqlTypeInfos = new SqlTypeInfos
            {
                new SqlTypeInfo("CHAR", true, false, false),
                new SqlTypeInfo("NCHAR", true, false, false),
                new SqlTypeInfo("VARCHAR", true),
                new SqlTypeInfo("NVARCHAR", true),
                new SqlTypeInfo("NTEXT"),

                new SqlTypeInfo("BIT"),
                new SqlTypeInfo("TINYINT"),
                new SqlTypeInfo("SMALLINT"),
                new SqlTypeInfo("INT"),
                new SqlTypeInfo("BIGINT"),

                new SqlTypeInfo("DECIMAL", true, true, false),
                new SqlTypeInfo("NUMERIC", true, true, false),
                new SqlTypeInfo("MONEY"),
                new SqlTypeInfo("SMALLMONEY"),

                new SqlTypeInfo("FLOAT"),
                new SqlTypeInfo("REAL"),

                new SqlTypeInfo("DATE"),
                new SqlTypeInfo("TIME", true, false, false),
                new SqlTypeInfo("DATETIME"),
                new SqlTypeInfo("DATETIME2", true, false, false),
                new SqlTypeInfo("DATETIMEOFFSET", true, false, false),
                new SqlTypeInfo("SMALLDATETIME"),

                new SqlTypeInfo("BINARY", true, false, false),
                new SqlTypeInfo("VARBINARY", true),
                new SqlTypeInfo("IMAGE"),

                new SqlTypeInfo("XML"),
            };
            TypesPerVersions.Add(new Configuration.MsSql2016(), sqlTypeInfos);
        }

        public static SqlTypeInfos Current
        {
            get
            {
                return TypesPerVersions[SqlEngines.GetLatestVersion<IMsSqlDialect>()];
            }
        }
    }
}