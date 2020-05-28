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

            var q = new Query
            {
                Table = db.Parent
            };

            var result = qb.Build(q);

            Assert.AreEqual("SELECT Id, Name FROM Parent", result);
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

            // q = q.Join(db.Parent);

            var result = qb.Build(q);

            Assert.AreEqual("SELECT Id, Name, ParentId, Name AS 'p_Name' FROM Child c\r\n\t" +
                "LEFT JOIN Parent p ON p.Id = c.ParentId", result);
        }
    }
}
