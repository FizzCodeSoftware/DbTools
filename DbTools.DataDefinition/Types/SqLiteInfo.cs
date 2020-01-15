namespace FizzCode.DbTools.DataDefinition
{
    using FizzCode.DbTools.Common;

    public class SqLiteInfo : DbInfo
    {
        public SqLiteInfo()
        {
            var sqlTypeInfos = new SqlTypeInfos
            {
                new SqlTypeInfo("INTEGER"),
                new SqlTypeInfo("REAL"),
                new SqlTypeInfo("TEXT"),
                new SqlTypeInfo("BLOB"),
            };
            TypesPerVersions.Add(new Common.SqLite3(), sqlTypeInfos);
        }

        public static SqlTypeInfos Current
        {
            get
            {
                return TypesPerVersions[SqlEngines.GetLatestVersion(SqlDialectX.SqLite)];
            }
        }
    }
}