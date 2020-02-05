namespace FizzCode.DbTools.DataDefinitionDocumenter.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UniqueNameTests
    {
        [TestMethod]
        public void GetUniqueNameTest()
        {
            var uniqueName = new UniqueName(4);

            var r1 = uniqueName.GetUniqueName("applA");
            var r2 = uniqueName.GetUniqueName("applB");

            Assert.AreEqual("appl", r1);
            Assert.AreEqual("app1", r2);
        }

        [TestMethod]
        public void GetUniqueNameTest2()
        {
            var uniqueName = new UniqueName(4);

            var r1 = uniqueName.GetUniqueName("applA");
            var r2 = uniqueName.GetUniqueName("applB");
            var r3 = uniqueName.GetUniqueName("applC");

            Assert.AreEqual("appl", r1);
            Assert.AreEqual("app1", r2);
            Assert.AreEqual("app2", r3);
        }

        [TestMethod]
        public void GetUniqueNameTest3()
        {
            var uniqueName = new UniqueName(4);

            var r1 = uniqueName.GetUniqueName("applA");
            var r2 = uniqueName.GetUniqueName("app1");
            var r3 = uniqueName.GetUniqueName("applC");

            Assert.AreEqual("appl", r1);
            Assert.AreEqual("app1", r2);
            Assert.AreEqual("app2", r3);
        }

        [TestMethod]
        public void GetUniqueNameTest4()
        {
            var uniqueName = new UniqueName(4);

            var r1 = uniqueName.GetUniqueName("applA");
            var r2 = uniqueName.GetUniqueName("applB");
            var r3 = uniqueName.GetUniqueName("app1");
            var r4 = uniqueName.GetUniqueName("applC");

            Assert.AreEqual("appl", r1);
            Assert.AreEqual("app1", r2);
            Assert.AreEqual("app2", r3);
            Assert.AreEqual("app3", r4);
        }

        [TestMethod]
        public void GetUniqueNameTest5()
        {
            var uniqueName = new UniqueName(4);

            var r1 = uniqueName.GetUniqueName("app1");
            var r2 = uniqueName.GetUniqueName("applA");
            var r3 = uniqueName.GetUniqueName("applB");
            var r4 = uniqueName.GetUniqueName("applC");

            Assert.AreEqual("app1", r1);
            Assert.AreEqual("appl", r2);
            Assert.AreEqual("app2", r3);
            Assert.AreEqual("app3", r4);
        }

        [TestMethod]
        public void GetUniqueNameTest6()
        {
            var uniqueName = new UniqueName(4);

            var r01 = uniqueName.GetUniqueName("ap11");
            var r02 = uniqueName.GetUniqueName("applA");
            var r03 = uniqueName.GetUniqueName("applB");
            var r04 = uniqueName.GetUniqueName("applC");
            var r05 = uniqueName.GetUniqueName("applD");
            var r06 = uniqueName.GetUniqueName("applE");
            var r07 = uniqueName.GetUniqueName("applF");
            var r08 = uniqueName.GetUniqueName("applG");
            var r09 = uniqueName.GetUniqueName("applH");
            var r10 = uniqueName.GetUniqueName("applI");
            var r11 = uniqueName.GetUniqueName("applJ");
            var r12 = uniqueName.GetUniqueName("applK");
            var r13 = uniqueName.GetUniqueName("applL");

            Assert.AreEqual("ap11", r01);
            Assert.AreEqual("appl", r02);
            Assert.AreEqual("app1", r03);
            Assert.AreEqual("app2", r04);
            Assert.AreEqual("app3", r05);
            Assert.AreEqual("app4", r06);
            Assert.AreEqual("app5", r07);
            Assert.AreEqual("app6", r08);
            Assert.AreEqual("app7", r09);
            Assert.AreEqual("app8", r10);
            Assert.AreEqual("app9", r11);
            Assert.AreEqual("ap10", r12);
            Assert.AreEqual("ap12", r13);
        }

        [TestMethod]
        public void GetUniqueNameTest_31Long()
        {
            var uniqueName = new UniqueName(31);

            var r01 = uniqueName.GetUniqueName("ANamePartThatIs31CharactersLong");
            var r02 = uniqueName.GetUniqueName("ANamePartThatIs31CharactersLong1");
            var r03 = uniqueName.GetUniqueName("ANamePartThatIs31CharactersLong2");
            var r04 = uniqueName.GetUniqueName("ANamePartThatIs31CharactersLong3");
            var r05 = uniqueName.GetUniqueName("ANamePartThatIs31CharactersLong4");
            var r06 = uniqueName.GetUniqueName("ANamePartThatIs31CharactersLong5");
            var r07 = uniqueName.GetUniqueName("ANamePartThatIs31CharactersLong6");
            var r08 = uniqueName.GetUniqueName("ANamePartThatIs31CharactersLong7");
            var r09 = uniqueName.GetUniqueName("ANamePartThatIs31CharactersLong8");
            var r10 = uniqueName.GetUniqueName("ANamePartThatIs31CharactersLong9");
            var r11 = uniqueName.GetUniqueName("ANamePartThatIs31CharactersLongA");

            Assert.AreEqual("ANamePartThatIs31CharactersLong", r01);
            Assert.AreEqual("ANamePartThatIs31CharactersLon1", r02);
            Assert.AreEqual("ANamePartThatIs31CharactersLon2", r03);
            Assert.AreEqual("ANamePartThatIs31CharactersLon3", r04);
            Assert.AreEqual("ANamePartThatIs31CharactersLon4", r05);
            Assert.AreEqual("ANamePartThatIs31CharactersLon5", r06);
            Assert.AreEqual("ANamePartThatIs31CharactersLon6", r07);
            Assert.AreEqual("ANamePartThatIs31CharactersLon7", r08);
            Assert.AreEqual("ANamePartThatIs31CharactersLon8", r09);
            Assert.AreEqual("ANamePartThatIs31CharactersLon9", r10);
            Assert.AreEqual("ANamePartThatIs31CharactersLo10", r11);
        }
    }
}