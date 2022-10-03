namespace FizzCode.DbTools.TestBase
{
    using System.Linq;
    using FizzCode.DbTools.DataDeclaration;

    public class TestDatabaseDeclaration : DatabaseDeclaration
    {
        protected TestDatabaseDeclaration()
            : base(new TestFactoryContainer(), MsSqlVersion.MsSql2016, new SqlEngineVersion[] { OracleVersion.Oracle12c, SqLiteVersion.SqLite3 })
        {
        }

        public void SetVersions(SqlEngineVersion mainVersion, SqlEngineVersion[] secondaryVersions = null)
        {
            MainVersion = mainVersion;
            SecondaryVersions = secondaryVersions?.ToList();
        }
    }
}