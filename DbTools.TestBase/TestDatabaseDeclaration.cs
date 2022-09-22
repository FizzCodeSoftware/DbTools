namespace FizzCode.DbTools.TestBase
{
    using FizzCode.DbTools.DataDeclaration;

    public class TestDatabaseDeclaration : DatabaseDeclaration
    {
        protected TestDatabaseDeclaration()
            : base(null, TypeMapperGetter.GetTypeMappers(MsSqlVersion.MsSql2016, OracleVersion.Oracle12c, SqLiteVersion.SqLite3))
        {
        }
    }
}