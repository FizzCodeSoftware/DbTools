namespace FizzCode.DbTools.DataGenerator.Tests
{
    using System;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Generic1;
    using FizzCode.DbTools.DataGenerator;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            var generator = new Generator(new GeneratorContext(new RandomBasic(0), new DateTime(2019, 7, 24)));
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
        public SqlTable Table { get; } = AddTable(table =>
        {
            table.AddInt32("Number");
            table.AddNVarChar("Text", 10);
        });
    }

    public class GeneratorTestDateDb : DatabaseDeclaration
    {
        public SqlTable Table { get; } = AddTable(table =>
        {
            table.AddDate("Date");
            table.AddDateTime("DateTime");
        });
    }

    public class GeneratorTestNameDb : DatabaseDeclaration
    {
        public SqlTable Table { get; } = AddTable(table =>
#pragma warning disable RCS1021 // Simplify lambda expression.
        {
            table.AddNVarChar("Name", 200).AddDataGenerator(new GeneratorName());
        });
#pragma warning restore RCS1021 // Simplify lambda expression.
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