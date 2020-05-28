namespace FizzCode.DbTools.QueryBuilder.Tests
{
    using FizzCode.DbTools.DataDefinition.Tests;
    using FizzCode.DbTools.QueryBuilder;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class QueryBuilderTests
    {
        [TestMethod]
        public void SimpleTable()
        {
            var db = new TestDatabaseFksTyped();

            var qb = new QueryBuilder();

            var q = new Query();
            q.Table = db.Parent;

            var result = qb.Build(q);

            Assert.AreEqual("SELECT * FROM Parent", result);
        }

        [TestMethod]
        public void BuildTest()
        {
            /* simple example
             * pull parent field with child
             */

            var db = new TestDatabaseFksTyped();

            var qb = new QueryBuilder();

            var q = new Query();
            q.Table = db.Child;

            var result = qb.Build(q);
        }
    }
}
