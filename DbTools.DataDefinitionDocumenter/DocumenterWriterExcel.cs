namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OfficeOpenXml;

    public class DocumenterWriterExcel : IDocumenterWriter
    {
        private ExcelPackage ExcelPackage { get; }

        public DocumenterWriterExcel()
        {
            ExcelPackage = new ExcelPackage();
        }

        private readonly Dictionary<string, string> _sheetNames = new Dictionary<string, string>();
        private readonly Dictionary<string, Sheet> _sheets = new Dictionary<string, Sheet>();

        protected string GetSheetName(string name)
        {
            if (!_sheetNames.ContainsKey(name))
                _sheetNames.Add(name, CreateUniqueSheetName(name));

            return _sheetNames[name];
        }

        protected string CreateUniqueSheetName(string name)
        {
            var uniqueName = name;
            if (name.Length > 31)
            {
                uniqueName = name.Substring(0, 31);
                var number = 1;
                while (_sheetNames.Any(i => i.Value.ToLower() == uniqueName.ToLower()))
                {
                    uniqueName = name.Substring(0, 31 - number.ToString().Length) + number++.ToString();
                }
            }

            return uniqueName;
        }

        public Sheet Sheet(string name)
        {
            var sheetName = GetSheetName(name);
            if (!_sheets.ContainsKey(sheetName))
                _sheets.Add(sheetName, new Sheet(ExcelPackage, sheetName));

            return _sheets[sheetName];
        }

        public void WriteLine(string name, params object[] content)
        {
            var sheetName = GetSheetName(name);
            Write(sheetName, content);

            Sheet(sheetName).LastRow++;
            Sheet(sheetName).LastColumn = 1;
        }

        public void Write(string name, params object[] content)
        {
            var sheetName = GetSheetName(name);

            foreach (var value in content)
            {
                Sheet(sheetName).SetValue(value);
                Sheet(sheetName).LastColumn++;
            }
        }

        public byte[] GetContent()
        {
            return ExcelPackage.GetAsByteArray();
        }
    }
}
