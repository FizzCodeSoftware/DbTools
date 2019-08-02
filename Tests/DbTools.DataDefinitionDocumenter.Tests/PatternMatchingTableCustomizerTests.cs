namespace FizzCode.DbTools.DataDefinitionDocumenter.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FizzCode.DbTools.DataDefinitionDocumenter;

    [TestClass]
    public class PatternMatchingTableCustomizerTests
    {
        [TestMethod]
        public void GetPatternMatching_StartsWith_Test()
        {
            var pm = new PatternMatchingTableCustomizer();
            pm.AddPattern("A*", true, null, null);

            Assert.IsTrue(pm.ShouldSkip("A"));
            Assert.IsTrue(pm.ShouldSkip("Aaa"));
            Assert.IsTrue(pm.ShouldSkip("Abb"));
            Assert.IsFalse(pm.ShouldSkip("B"));
            Assert.IsFalse(pm.ShouldSkip("Baa"));
            Assert.IsFalse(pm.ShouldSkip("Bbb"));
        }

        [TestMethod]
        public void GetPatternMatching_Contains_Test()
        {
            var pm = new PatternMatchingTableCustomizer();
            pm.AddPattern("*apple*", true, null, null);

            Assert.IsTrue(pm.ShouldSkip("xapplex"));
            Assert.IsTrue(pm.ShouldSkip("apple"));
            Assert.IsTrue(pm.ShouldSkip("applex"));

            Assert.IsTrue(pm.ShouldSkip("The quick brown fox jumps over the lazy apple dog"));

            Assert.IsFalse(pm.ShouldSkip("appl"));
            Assert.IsFalse(pm.ShouldSkip("pple"));
            Assert.IsFalse(pm.ShouldSkip("xxx"));
        }

        [TestMethod]
        public void GetPatternMatching_Dot_Test()
        {
            var pm = new PatternMatchingTableCustomizer();
            pm.AddPattern("ap?le", true, null, null);

            Assert.IsTrue(pm.ShouldSkip("apple"));
            Assert.IsTrue(pm.ShouldSkip("apxle"));

            Assert.IsTrue(pm.ShouldSkip("The quick brown fox jumps over the lazy apXle dog"));

            Assert.IsFalse(pm.ShouldSkip("appl"));
            Assert.IsFalse(pm.ShouldSkip("pple"));
            Assert.IsFalse(pm.ShouldSkip("xxx"));
        }
    }
}