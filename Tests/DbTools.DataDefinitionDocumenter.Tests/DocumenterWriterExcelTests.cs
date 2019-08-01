namespace FizzCode.DbTools.DataDefinitionDocumenter.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FizzCode.DbTools.DataDefinitionDocumenter;

    [TestClass]
    public class DocumenterWriterExcelTests
    {
        [TestMethod]
        public void SheetSameNames31CharsTest()
        {
            var dw = new DocumenterWriterExcel();

            const string thisNameIs31Chars = "ThisNameIsThirtyOneCharacterAaa";

            dw.Sheet(thisNameIs31Chars + "One");
            dw.Sheet(thisNameIs31Chars + "Two");
            dw.Sheet(thisNameIs31Chars + "Three");
            dw.Sheet(thisNameIs31Chars + "Four");
            dw.Sheet(thisNameIs31Chars + "Five");
            dw.Sheet(thisNameIs31Chars + "Six");
            dw.Sheet(thisNameIs31Chars + "Seven");
            dw.Sheet(thisNameIs31Chars + "Eight");
            dw.Sheet(thisNameIs31Chars + "Nine");
            dw.Sheet(thisNameIs31Chars + "Ten");
            dw.Sheet(thisNameIs31Chars + "TEle3ven");
        }

        [TestMethod]
        public void SheetSameNames31CharsCaseTest()
        {
            var dw = new DocumenterWriterExcel();

            const string thisNameIs31Chars_1 = "ThisNameIsThirtyOneCharacterAaa";
            const string thisNameIs31Chars_2 = "ThisNameIsTHIRTYOneCharacterAaa";

            dw.Sheet(thisNameIs31Chars_1 + "One");
            dw.Sheet(thisNameIs31Chars_1 + "Two");
            dw.Sheet(thisNameIs31Chars_2 + "One");
            dw.Sheet(thisNameIs31Chars_2 + "Two");
        }
    }
}