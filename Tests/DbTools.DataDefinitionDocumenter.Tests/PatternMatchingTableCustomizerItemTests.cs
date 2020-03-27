namespace FizzCode.DbTools.DataDefinitionDocumenter.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PatternMatchingTableCustomizerItemTests
    {
        [TestMethod]
        public void Equals()
        {
            var item1 = new PatternMatchingTableCustomizerItem(null, null, false, null, null);
            var item2 = new PatternMatchingTableCustomizerItem(null, null, false, null, null);

            Assert.IsTrue(item1.Equals(item2));
            Assert.IsTrue(item2.Equals(item1));
        }
    }
}