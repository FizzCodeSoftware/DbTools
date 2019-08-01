namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using OfficeOpenXml;

    public class Sheet
    {
        public Sheet(ExcelPackage excelPackage, string name)
        {
            ExcelWorksheet = excelPackage.Workbook.Worksheets.Add(name);
            LastRow = 1;
            LastColumn = 1;
        }

        public ExcelWorksheet ExcelWorksheet { get; }

        public int LastRow { get; set; }
        public int LastColumn { get; set; }

        public void SetValue(object value)
        {
            ExcelWorksheet.SetValue(LastRow, LastColumn, value);
        }
    }
}
