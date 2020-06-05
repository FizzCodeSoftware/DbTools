namespace FizzCode.DbTools.QueryBuilder.Tests
{
    using System.Linq;
    using FizzCode.DbTools.DataDefinition;
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

            Assert.AreEqual("SELECT p.Id, p.Name\r\nFROM Parent p", result);
        }

        [TestMethod]
        public void JoinWithAlias()
        {
            var db = new TestDatabaseFksTyped();
            var qb = new QueryBuilder();
            var q = new Query(db.Child)
                .Join(db.Parent, "ppp");

            var result = qb.Build(q);

            Assert.AreEqual("SELECT c.Id, c.Name, c.ParentId, ppp.Id AS 'ppp_Id', ppp.Name AS 'ppp_Name'\r\n"
                + "FROM Child c\r\n" +
                "LEFT JOIN Parent ppp ON ppp.Id = c.ParentId", result);
        }

        [TestMethod]
        public void JoinColumnsNone()
        {
            var db = new TestDatabaseFksTyped();
            var qb = new QueryBuilder();
            var q = new Query(db.Child)
                .JoinInner(db.Parent, new None());

            var result = qb.Build(q);

            Assert.AreEqual("SELECT c.Id, c.Name, c.ParentId\r\n"
                + "FROM Child c\r\n" +
                "INNER JOIN Parent p ON p.Id = c.ParentId", result);
        }

        [TestMethod]
        public void ParentFieldWithChild()
        {
            var db = new TestDatabaseFksTyped();
            var qb = new QueryBuilder();
            var q = new Query(db.Child)
                .Join(db.Parent, db.Parent.Name);

            var result = qb.Build(q);

            Assert.AreEqual("SELECT c.Id, c.Name, c.ParentId, p.Name AS 'p_Name'\r\nFROM Child c\r\n" +
                "LEFT JOIN Parent p ON p.Id = c.ParentId", result);
        }

        [TestMethod]
        public void SimpleTableColumns()
        {
            var db = new TestDatabaseFksTyped();
            var qb = new QueryBuilder();
            var q = new Query(db.Child, db.Child.Name, db.Child.ParentId);

            var result = qb.Build(q);

            Assert.AreEqual("SELECT c.Name, c.ParentId\r\nFROM Child c", result);
        }

        [TestMethod]
        public void JoinSameTable()
        {
            var db = new TestDatabaseFksTyped();
            var qb = new QueryBuilder();
            var q = new Query(db.Child)
                .Join(db.Parent, "p1")
                .Join(db.Parent, "p2");

            var result = qb.Build(q);

            Assert.AreEqual("SELECT c.Id, c.Name, c.ParentId, p1.Id AS 'p1_Id', p1.Name AS 'p1_Name', p2.Id AS 'p2_Id', p2.Name AS 'p2_Name'\r\n" +
                "FROM Child c\r\n" +
                "LEFT JOIN Parent p1 ON p1.Id = c.ParentId\r\n" +
                "LEFT JOIN Parent p2 ON p2.Id = c.ParentId", result);
        }
    }
}
