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
            var q = new Query(db.Parent);

            var result = qb.Build(q);

            Assert.AreEqual("SELECT Id, Name\r\nFROM Parent p", result);
        }

        [TestMethod]
        public void ParentFieldWithChild()
        {
            var db = new TestDatabaseFksTyped();
            var qb = new QueryBuilder();
            var q = new Query(db.Child).Join(db.Parent, db.Parent.Name);

            var result = qb.Build(q);

            Assert.AreEqual("SELECT Id, Name, ParentId, p.Name AS 'p_Name'\r\nFROM Child c\r\n" +
                "LEFT JOIN Parent p ON p.Id = c.ParentId", result);
        }
    }
}
