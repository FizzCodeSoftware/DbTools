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

            var r1 = uniqueName.GetUniqueName("appl");
            var r2 = uniqueName.GetUniqueName("appl");

            Assert.AreEqual("appl", r1);
            Assert.AreEqual("app1", r2);
        }
    }
}