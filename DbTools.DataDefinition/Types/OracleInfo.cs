namespace FizzCode.DbTools.DataDefinition
{
    using FizzCode.DbTools.Common;

    public class OracleInfo : DbInfo
    {
        public OracleInfo()
        {
            var sqlTypeInfos = new SqlTypeInfos
            {
                new SqlTypeInfo("CHAR", true, false, false),
                new SqlTypeInfo("NCHAR", true, false, false),
                new SqlTypeInfo("VARCHAR", true, false, false),
                new SqlTypeInfo("NVARCHAR", true, false, false),
                new SqlTypeInfo("VARCHAR2", true, false, false),
                new SqlTypeInfo("NVARCHAR2", true, false, false),

                new SqlTypeInfo("BLOB", false, false, false),
                new SqlTypeInfo("CLOB", false, false, false),
                new SqlTypeInfo("NCLOB", false, false, false),
                new SqlTypeInfo("BFILE", false, false, false),
                new SqlTypeInfo("LONG", false, false, true),
                new SqlTypeInfo("LONG RAW", false, false, true),
                // BINARY, VARBINARY
                // INTERVAL 
                // TIME
                // TT*

                new SqlTypeInfo("NUMBER", true, false, true, false, true),

                new SqlTypeInfo("BINARY_FLOAT"),
                new SqlTypeInfo("BINARY_DOUBLE"),

                new SqlTypeInfo("DATE"),
                new SqlTypeInfo("TIMESTAMP WITH TIME ZONE"),
                new SqlTypeInfo("TIMESTAMP WITH LOCAL TIME ZONE"),

                new SqlTypeInfo("XMLType"),
                new SqlTypeInfo("UriType")
            };

            TypesPerVersions.Add(new Common.Oracle12c(), sqlTypeInfos);
        }

        public static SqlTypeInfos Current
        {
            get
            {
                return TypesPerVersions[SqlEngines.GetLatestVersion(SqlDialectX.Oracle)];
            }
        }
    }
}