namespace FizzCode.DbTools.DataDefinition
{
    using FizzCode.DbTools.Configuration;

    public class SqLiteInfo : DbInfo
    {
        static SqLiteInfo()
        {
            var sqlTypeInfos = new SqlTypeInfos
            {
                new SqlTypeInfo("INTEGER"),
                new SqlTypeInfo("REAL"),
                new SqlTypeInfo("TEXT"),
                new SqlTypeInfo("BLOB"),
            };
            TypesPerVersions.Add(new Configuration.SqLite3(), sqlTypeInfos);
        }

        public static SqlTypeInfos Current
        {
            get
            {
                return TypesPerVersions[SqlEngines.GetLatestVersion<ISqLiteDialect>()];
            }
        }
    }
}