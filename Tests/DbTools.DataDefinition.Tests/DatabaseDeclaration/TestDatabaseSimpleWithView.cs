namespace FizzCode.DbTools.DataDefinition.Tests
{
    using FizzCode.DbTools.DataDefinition.Base;
    using FizzCode.DbTools.DataDefinition.Generic1;
    using FizzCode.DbTools.QueryBuilder;
    using FizzCode.DbTools.TestBase;

    public class TestDatabaseSimpleWithView : TestDatabaseDeclaration
    {
        public TestDatabaseSimpleWithView()
            : base()
            //: base(new TestFactoryContainer(), null, new SqlEngineVersion[] { MsSqlVersion.MsSql2016, OracleVersion.Oracle12c, SqLiteVersion.SqLite3 })
        {
        }

        public SqlTable Company { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddNVarChar("Name", 100);
        });

        public SqlView CompanyView => new ViewFromQuery(new Query(Company));
    }
}
