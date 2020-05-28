namespace FizzCode.DbTools.QueryBuilder.Tests
{
    using FizzCode.DbTools.DataDefinition.Tests;
    using FizzCode.DbTools.QueryBuilder;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class QueryBuilderTests
    {
        [TestMethod]
        public void BuildTest()
        {
            /* simple example
             * pull parent field with child
             */

            var db = new TestDatabaseFks();

            var qb = new QueryBuilder();

            var q = new Query();
            // q.Table = db()

            var result = qb.Build(q);
        }
    }
}
