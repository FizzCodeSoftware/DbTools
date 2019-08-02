using System.Drawing;

namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    public interface IDocumenterWriter
    {
        void Write(string sheetName, params object[] content);
        void WriteLine(string sheetName, params object[] content);

        void Write(Color? backgroundColor, string sheetName, params object[] content);
        void WriteLine(Color? backgroundColor, string sheetName, params object[] content);

        byte[] GetContent();

        void WriteLink(string sheetName, string targetSheetName);
        void WriteLink(Color? backgroundColor, string sheetName, string targetSheetName);
    }
}
