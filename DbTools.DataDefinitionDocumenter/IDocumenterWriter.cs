namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionGenerator;
    using OfficeOpenXml;

    public interface IDocumenterWriter
    {
        void Write(string sheetName, params object[] content);
        void WriteLine(string sheetName, params object[] content);

        byte[] GetContent();
    }
}
