namespace FizzCode.DbTools.TestBase
{
    using FizzCode.DbTools.DataDeclaration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Factory;

    public class TestDatabaseDeclaration : DatabaseDeclaration
    {
        protected TestDatabaseDeclaration()
            : base(null, new[] { MsSqlVersion.MsSql2016.GetTypeMapper(), OracleVersion.Oracle12c.GetTypeMapper(), SqLiteVersion.SqLite3.GetTypeMapper() })
        {
        }
    }
}