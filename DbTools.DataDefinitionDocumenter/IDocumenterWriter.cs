namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System.Drawing;

    public interface IDocumenterWriter
    {
        void SetSheetColor(string sheetName, Color color);

        void Write(string sheetName, params object[] content);
        void WriteLine(string sheetName, params object[] content);

        void Write(Color? backgroundColor, string sheetName, params object[] content);
        void WriteAndMerge(Color? backgroundColor, string sheetName, int mergeAmount, object content);
        void WriteLine(Color? backgroundColor, string sheetName, params object[] content);

        byte[] GetContent();

        void WriteLink(string sheetName, string text, string targetSheetName, Color? backgroundColor = null);
    }
}
