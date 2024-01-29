using FizzCode.DbTools.DataDefinitionDocumenter.Excel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FizzCode.DbTools.DataDefinitionDocumenter.Tests
{
    [TestClass()]
    public class NamingHelperTests
    {
        [TestMethod()]
        public void QuoteSheetNameIfNeedeedTest()
        {
            const string thisNameContainsDollarSign = "ThisName$ContainsDollarSign";
            var result_thisNameContainsDollarSign = NamingHelper.QuoteSheetNameIfNeedeed(thisNameContainsDollarSign);
            Assert.AreEqual("'" + thisNameContainsDollarSign + "'", result_thisNameContainsDollarSign);

            const string thisNameDoesNotContainsDollarSign = "ThisNameDoesNotContainsDollarSign";
            var result_thisNameDoesNotContainsDollarSign = NamingHelper.QuoteSheetNameIfNeedeed(thisNameDoesNotContainsDollarSign);
            Assert.AreEqual(thisNameDoesNotContainsDollarSign, result_thisNameDoesNotContainsDollarSign);
        }
    }
}