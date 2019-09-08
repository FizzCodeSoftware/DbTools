#pragma warning disable RCS1077 // Optimize LINQ method call.
namespace FizzCode.DbTools.DataDefinition.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.DataDefinition;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class TestDatabaseOtherChainIsCircular : DatabaseDeclaration
    {
        public static LazySqlTable Start = new LazySqlTable(() =>
        {
            var table = new SqlTable();
            table.AddInt32("StartId").SetPK().SetIdentity();
            table.AddForeignKey(nameof(FK1));
            return table;
        });

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

    public class TestDatabaseOtherTwoChainsAreCircular : DatabaseDeclaration
    {
        public static LazySqlTable Start = new LazySqlTable(() =>
        {
            var table = new SqlTable();
            table.AddInt32("StartId").SetPK().SetIdentity();
            table.AddForeignKey(nameof(FKA1));
            table.AddForeignKey(nameof(FKB1));
            return table;
        });

        public static LazySqlTable FKA1 = new LazySqlTable(() =>
        {
            var table = new SqlTable();
            table.AddInt32("FKA1Id").SetPK().SetIdentity();
            table.AddForeignKey(nameof(FKA2));
            return table;
        });

        public static LazySqlTable FKA2 = new LazySqlTable(() =>
        {
            var table = new SqlTable();
            table.AddInt32("FKA2Id").SetPK().SetIdentity();
            table.AddForeignKey(nameof(FKA1));
            return table;
        });

        public static LazySqlTable FKB1 = new LazySqlTable(() =>
        {
            var table = new SqlTable();
            table.AddInt32("FKB1Id").SetPK().SetIdentity();
            table.AddForeignKey(nameof(FKB2));
            return table;
        });

        public static LazySqlTable FKB2 = new LazySqlTable(() =>
        {
            var table = new SqlTable();
            table.AddInt32("FKB2Id").SetPK().SetIdentity();
            table.AddForeignKey(nameof(FKB1));
            return table;
        });
    }

    [TestClass]
    public class DatabaseDefinitionCircularFKDetectionTests
    {
        [TestMethod]
        public void TestDatabaseOtherChainIsCircular()
        {
            var db = new TestDatabaseOtherChainIsCircular();

            var startCfks = db.GetTable("Start").Properties.OfType<CircularFK>().ToList();
            var fk1Cfks = db.GetTable("FK1").Properties.OfType<CircularFK>().ToList();
            var fk2Cfks = db.GetTable("FK2").Properties.OfType<CircularFK>().ToList();

            Assert.AreEqual(1, startCfks.Count());
            Assert.AreEqual(1, fk1Cfks.Count());
            Assert.AreEqual(1, fk2Cfks.Count());

            Assert.AreEqual(3, startCfks[0].ForeignKeyChain.Count());
            CheckeOtherChainIsCircular(db.GetTable("Start").Properties.OfType<ForeignKey>().First(), db.GetTable("FK1"), db.GetTable("FK2"), startCfks[0], fk1Cfks, fk2Cfks);
        }

        [TestMethod]
        public void TestDatabaseOtherTwoChainsAreCircular()
        {
            var db = new TestDatabaseOtherTwoChainsAreCircular();

            var startCfks = db.GetTable("Start").Properties.OfType<CircularFK>().ToList();
            var fka1Cfks = db.GetTable("FKA1").Properties.OfType<CircularFK>().ToList();
            var fka2Cfks = db.GetTable("FKA2").Properties.OfType<CircularFK>().ToList();

            var fkb1Cfks = db.GetTable("FKB1").Properties.OfType<CircularFK>().ToList();
            var fkb2Cfks = db.GetTable("FKB2").Properties.OfType<CircularFK>().ToList();

            Assert.AreEqual(2, startCfks.Count());
            Assert.AreEqual(1, fka1Cfks.Count());
            Assert.AreEqual(1, fka2Cfks.Count());
            Assert.AreEqual(1, fkb1Cfks.Count());
            Assert.AreEqual(1, fkb2Cfks.Count());

            Assert.AreEqual(3, startCfks[0].ForeignKeyChain.Count());
            CheckeOtherChainIsCircular(db.GetTable("Start").Properties.OfType<ForeignKey>().First(), db.GetTable("FKA1"), db.GetTable("FKA2"), startCfks[0], fka1Cfks, fka2Cfks);
            Assert.AreEqual(3, startCfks[1].ForeignKeyChain.Count());
            CheckeOtherChainIsCircular(db.GetTable("Start").Properties.OfType<ForeignKey>().ToList()[1], db.GetTable("FKB1"), db.GetTable("FKB2"), startCfks[1], fkb1Cfks, fkb2Cfks);
        }

        private static void CheckeOtherChainIsCircular(ForeignKey startFk, SqlTable fk1, SqlTable fk2, CircularFK startCfk, List<CircularFK> fk1Cfks, List<CircularFK> fk2Cfks)
        {
            Assert.AreEqual(2, fk1Cfks[0].ForeignKeyChain.Count());
            Assert.AreEqual(2, fk2Cfks[0].ForeignKeyChain.Count());

            Assert.AreEqual(startFk, startCfk.ForeignKeyChain[0]);

            Assert.AreEqual(fk1.Properties.OfType<ForeignKey>().First()
                  , startCfk.ForeignKeyChain[1]);

            Assert.AreEqual(fk2.Properties.OfType<ForeignKey>().First()
                  , startCfk.ForeignKeyChain[2]);

            Assert.AreEqual(fk1.Properties.OfType<ForeignKey>().First()
                , fk1Cfks[0].ForeignKeyChain[0]);

            Assert.AreEqual(fk2.Properties.OfType<ForeignKey>().First()
                , fk1Cfks[0].ForeignKeyChain[1]);

            Assert.AreEqual(fk2.Properties.OfType<ForeignKey>().First()
                , fk2Cfks[0].ForeignKeyChain[0]);

            Assert.AreEqual(fk1.Properties.OfType<ForeignKey>().First()
                , fk2Cfks[0].ForeignKeyChain[1]);
        }

        [TestMethod]
        public void TestDatabaseSimpleNoNameProvided()
        {
            var tables = new TestDatabaseSimpleNoNameProvided().GetTables();

            var cfks = tables[0].Properties.OfType<CircularFK>().Count();

            Assert.AreEqual(0, cfks);
        }

        [TestMethod]
        public void TestDatabaseSimple()
        {
            var tables = new TestDatabaseSimple().GetTables();

            var cfks = tables[0].Properties.OfType<CircularFK>().Count();

            Assert.AreEqual(0, cfks);
        }

        [TestMethod]
        public void TestDatabaseFks()
        {
            var dd = new TestDatabaseFks();

            var parent = dd.GetTable("Parent");
            var child = dd.GetTable("Child");
            var childChild = dd.GetTable("ChildChild");

            var parentCFKs = parent.Properties.OfType<CircularFK>().Count();
            var childCFKs = child.Properties.OfType<CircularFK>().Count();
            var childChildCFKs = childChild.Properties.OfType<CircularFK>().Count();

            Assert.AreEqual(0, parentCFKs);
            Assert.AreEqual(0, childCFKs);
            Assert.AreEqual(0, childChildCFKs);
        }

        [TestMethod]
        public void TestDatabaseCircular2FK()
        {
            var dd = new TestDatabaseCircular2FK();

            var a = dd.GetTable("A");
            var b = dd.GetTable("B");

            Assert.AreEqual(1, a.Properties.OfType<CircularFK>().Count());
            Assert.AreEqual(1, b.Properties.OfType<CircularFK>().Count());

            var aCFK = a.Properties.OfType<CircularFK>().First();
            var bCFK = b.Properties.OfType<CircularFK>().First();

            Assert.AreEqual("A", aCFK.SqlTable.SchemaAndTableName.TableName);
            Assert.AreEqual("B", bCFK.SqlTable.SchemaAndTableName.TableName);

            Assert.AreEqual(2, aCFK.ForeignKeyChain.Count);
            Assert.AreEqual(2, bCFK.ForeignKeyChain.Count);

            Assert.AreEqual("A", aCFK.ForeignKeyChain.First().SqlTable.SchemaAndTableName.TableName);
            Assert.AreEqual("B", aCFK.ForeignKeyChain.First().ReferredTable.SchemaAndTableName.TableName);

            Assert.AreEqual("B", aCFK.ForeignKeyChain.Last().SqlTable.SchemaAndTableName.TableName);
            Assert.AreEqual("A", aCFK.ForeignKeyChain.Last().ReferredTable.SchemaAndTableName.TableName);

            Assert.AreEqual("B", bCFK.ForeignKeyChain.First().SqlTable.SchemaAndTableName.TableName);
            Assert.AreEqual("A", bCFK.ForeignKeyChain.First().ReferredTable.SchemaAndTableName.TableName);

            Assert.AreEqual("A", bCFK.ForeignKeyChain.Last().SqlTable.SchemaAndTableName.TableName);
            Assert.AreEqual("B", bCFK.ForeignKeyChain.Last().ReferredTable.SchemaAndTableName.TableName);
        }

        [TestMethod]
        public void TestDatabaseCircular3FK()
        {
            var dd = new TestDatabaseCircular3FK();

            var a = dd.GetTable("A");
            var b = dd.GetTable("B");
            var c = dd.GetTable("C");

            var aCFK = a.Properties.OfType<CircularFK>().First();
            var bCFK = b.Properties.OfType<CircularFK>().First();
            var cCFK = c.Properties.OfType<CircularFK>().First();

            Assert.AreEqual("A", aCFK.SqlTable.SchemaAndTableName.TableName);
            Assert.AreEqual("B", bCFK.SqlTable.SchemaAndTableName.TableName);
            Assert.AreEqual("C", cCFK.SqlTable.SchemaAndTableName.TableName);

            Assert.AreEqual(3, aCFK.ForeignKeyChain.Count);
            Assert.AreEqual(3, bCFK.ForeignKeyChain.Count);
            Assert.AreEqual(3, cCFK.ForeignKeyChain.Count);

            Assert.AreEqual("A", aCFK.ForeignKeyChain.First().SqlTable.SchemaAndTableName.TableName);
            Assert.AreEqual("C", aCFK.ForeignKeyChain.Last().SqlTable.SchemaAndTableName.TableName);

            Assert.AreEqual("B", bCFK.ForeignKeyChain.First().SqlTable.SchemaAndTableName.TableName);
            Assert.AreEqual("A", bCFK.ForeignKeyChain.Last().SqlTable.SchemaAndTableName.TableName);

            Assert.AreEqual("C", cCFK.ForeignKeyChain.First().SqlTable.SchemaAndTableName.TableName);
            Assert.AreEqual("B", cCFK.ForeignKeyChain.Last().SqlTable.SchemaAndTableName.TableName);
        }

        [TestMethod]
        public void TestDatabaseSelfFK()
        {
            var dd = new TestDatabaseSelfFK();

            var company = dd.GetTable("Company");
        }
    }
}
#pragma warning restore RCS1077 // Optimize LINQ method call.