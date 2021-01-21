namespace FizzCode.DbTools.TestBase
{
    using FizzCode.DbTools.DataDefinition;

    public class TestDatabaseDeclaration : DatabaseDeclaration
    {
        protected TestDatabaseDeclaration()
            : base(null, new[] { MsSqlVersion.MsSql2016.GetTypeMapper(), OracleVersion.Oracle12c.GetTypeMapper(), SqLiteVersion.SqLite3.GetTypeMapper() })
        {
        }
    }
}