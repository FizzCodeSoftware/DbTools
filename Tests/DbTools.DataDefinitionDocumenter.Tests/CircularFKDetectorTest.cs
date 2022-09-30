namespace FizzCode.DbTools.DataDefinitionDocumenter.Tests
{
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Base;
    using FizzCode.DbTools.DataDefinition.Generic1;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /* Circular reference with 3 tables
     * A <- B
     * B <- C
     * C <- A
     * 
     * Reference outside of the circular reference
     * B <- X
     * (no table points to X)
     */

    public class CircularDdABC_X : TestDatabaseDeclaration
    {
        public SqlTable A { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK();
            table.AddInt32("CId").SetForeignKeyToTable("C");
        });

        public SqlTable B { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK();
            table.AddInt32("AId").SetForeignKeyToTable("A");
        });

        public SqlTable X { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK();
            table.AddInt32("BId").SetForeignKeyToTable("B");
        });

        public SqlTable C { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK();
            table.AddInt32("BId").SetForeignKeyToTable("B");
        });
    }

    public class CircularDdA0B1C1_B2C2 : TestDatabaseDeclaration
    {
        public SqlTable A0 { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK();
            table.AddInt32("C1Id").SetForeignKeyToTable("C1");
            table.AddInt32("C2Id").SetForeignKeyToTable("C2");
        });

        public SqlTable B1 { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK();
            table.AddInt32("A0Id").SetForeignKeyToTable("A0");
        });

        public SqlTable C1 { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK();
            table.AddInt32("B1Id").SetForeignKeyToTable("B1");
        });

        public SqlTable B2 { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK();
            table.AddInt32("A0Id").SetForeignKeyToTable("A0");
        });

        public SqlTable C2 { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK();
            table.AddInt32("B2Id").SetForeignKeyToTable("B2");
        });
    }

    public class CircularDdAB_CD : TestDatabaseDeclaration
    {
        public SqlTable A { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK();
        });

        public SqlTable B { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK();
            table.AddInt32("AId").SetForeignKeyToTable("A");
        });

        public SqlTable C { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK();
        });

        public SqlTable D { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK();
            table.AddInt32("CId").SetForeignKeyToTable("C");
        });
    }

    [TestClass]
    public class CircularFKDetectorTest
    {
        [TestMethod]
        [LatestSqlVersions]
        public void CircularFKDetectorABC_X(SqlEngineVersion version)
        {
            var dd = new CircularDdABC_X();

            var documenter = new Documenter(DataDefinitionDocumenterTestsHelper.CreateTestDocumenterContext(version), version, "CircularDdABC_X");

            documenter.Document(dd);

            // TODO move to dd test
            // CircularFKDetector.DectectCircularFKs(dd.GetTables());
            // var cs = dd.GetTables().Select(t => t.Properties.OfType<CircularFK>()).ToList();
        }

        [TestMethod]
        [LatestSqlVersions]
        public void CircularFKDetectorAB_CD(SqlEngineVersion version)
        {
            var dd = new CircularDdAB_CD();

            var documenter = new Documenter(DataDefinitionDocumenterTestsHelper.CreateTestDocumenterContext(version), version, "CircularFKDetectorAB_CD");

            documenter.Document(dd);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void CircularDdA0B1C1_B2C2(SqlEngineVersion version)
        {
            var dd = new CircularDdA0B1C1_B2C2();

            var documenter = new Documenter(DataDefinitionDocumenterTestsHelper.CreateTestDocumenterContext(version), version, "CircularDdA0B1C1_B2C2");

            documenter.Document(dd);
        }
    }
}