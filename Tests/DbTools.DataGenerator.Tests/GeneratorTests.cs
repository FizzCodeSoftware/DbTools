namespace FizzCode.DbTools.DataGenerator.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataGenerator;

    [TestClass]
    public class GeneratorTests
    {
        [TestMethod]
        public void Simple()
        {
            var generator = new Generator(new GeneratorContext(new RandomBasic(0)));
            var row = generator.Generate(new GeneratorTestSimpleDb().GetTable("Table"));

            Assert.IsTrue(row.GetAs<int>("Number") > -1);
            Assert.IsFalse(string.IsNullOrEmpty(row.GetAs<string>("Text")));
        }

        [TestMethod]
        public void Date()
        {
            var generator = new Generator(new GeneratorContext(new RandomBasic(0), new DateTime(2019,7,24)));
            var row = generator.Generate(new GeneratorTestDateDb().GetTable("Table"));

            Assert.IsTrue(row["Date"] != null);
            Assert.IsTrue(row["DateTime"] != null);

            Assert.IsTrue(row.GetAs<DateTime>("Date").Year > 0);
            Assert.IsTrue(row.GetAs<DateTime>("Date").Month > 0);
            Assert.IsTrue(row.GetAs<DateTime>("Date").Day > 0);

            Assert.IsTrue(row.GetAs<DateTime>("Date").Hour == 0);
            Assert.IsTrue(row.GetAs<DateTime>("Date").Minute == 0);
            Assert.IsTrue(row.GetAs<DateTime>("Date").Second == 0);

            Assert.IsTrue(row.GetAs<DateTime>("DateTime").Year > 0);
            Assert.IsTrue(row.GetAs<DateTime>("DateTime").Month > 0);
            Assert.IsTrue(row.GetAs<DateTime>("DateTime").Day > 0);
        }

        [TestMethod]
        public void Name()
        {
            var generator = new Generator(new GeneratorContext(new RandomBasic(0), new DateTime(2019, 7, 24)));
            var row = generator.Generate(new GeneratorTestNameDb().GetTable("Table"));

            Assert.IsFalse(string.IsNullOrEmpty(row.GetAs<string>("Name")));
        }
    }

    public class GeneratorTestSimpleDb : DatabaseDeclaration
    {
        public static LazySqlTable Table = new LazySqlTable(() =>
        {
            var table = new SqlTable();
            table.AddInt32("Number");
            table.AddNVarChar("Text", 10);
            return table;
        });
    }

    public class GeneratorTestDateDb : DatabaseDeclaration
    {
        public static LazySqlTable Table = new LazySqlTable(() =>
        {
            var table = new SqlTable();
            table.AddDate("Date");
            table.AddDateTime("DateTime");
            return table;
        });
    }

    public class GeneratorTestNameDb : DatabaseDeclaration
    {
        public static LazySqlTable Table = new LazySqlTable(() =>
        {
            var table = new SqlTable();
            table.AddNVarChar("Name", 200).AddDataGenerator(new GeneratorName());
            return table;
        });
    }

    public static class GeneratorHelper
    {
        public static SqlColumn AddDataGenerator(this SqlColumn column, GeneratorBase generator)
        {
            column.Properties.Add(new SqlColumnDataGenerator(column, generator));
            return column;
        }
    }
}