namespace FizzCode.DbTools.DataDefinition.Tests
{
    using System.Linq;
    using FizzCode.DbTools.DataDefinition;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class TestDatabaseCircularFKSetPK : DatabaseDeclaration
    {
        public static LazySqlTable FK1 = new LazySqlTable(() =>
        {
            var table = new SqlTable();
            table.AddInt32("FK1Id").SetPK().SetIdentity();
            table.AddForeignKey(nameof(FK2));
            return table;
        });

        public static LazySqlTable FK2 = new LazySqlTable(() =>
        {
            var table = new SqlTable();
            table.AddInt32("FK2Id").SetPK().SetIdentity();
            table.AddForeignKey(nameof(FK1));
            return table;
        });
    }

    [TestClass]
    public class DatabaseDefinitionCircularFKWithSetFK
    {
        [TestMethod]
        public void TestDatabaseDefinitionCircularFKWithSetFK()
        {
            var db = new TestDatabaseCircularFKSetPK();

            var fk1Cfks = db.GetTable("FK1").Properties.OfType<CircularFK>().ToList();
            var fk2Cfks = db.GetTable("FK2").Properties.OfType<CircularFK>().ToList();

            Assert.AreEqual(1, fk1Cfks.Count);
            Assert.AreEqual(1, fk2Cfks.Count);
        }
    }
}
