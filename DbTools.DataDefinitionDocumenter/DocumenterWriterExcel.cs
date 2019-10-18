namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using OfficeOpenXml;
    using OfficeOpenXml.Style;

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
                while (_sheetNames.Any(i => string.Equals(i.Value, uniqueName, StringComparison.OrdinalIgnoreCase)))
                {
                    uniqueName = name.Substring(0, 31 - number.ToString(CultureInfo.InvariantCulture).Length) + number++.ToString(CultureInfo.InvariantCulture);
                }
            }

            return uniqueName;
        }

        public Sheet Sheet(string name)
        {
            return Sheet(name, null);
        }

        public Sheet Sheet(string name, Color? backGroundColor)
        {
            var sheetName = GetSheetName(name);
            if (!_sheets.ContainsKey(sheetName))
                _sheets.Add(sheetName, new Sheet(ExcelPackage, sheetName, backGroundColor));

            return _sheets[sheetName];
        }

        public void WriteLine(string sheetName, params object[] content)
        {
            sheetName = GetSheetName(sheetName);
            Write(sheetName, content);

            Sheet(sheetName).LastRow++;
            Sheet(sheetName).LastColumn = 1;
        }

        public void WriteLine(Color? backgroundColor, string sheetName, params object[] content)
        {
            sheetName = GetSheetName(sheetName);
            Write(backgroundColor, sheetName, content);

            Sheet(sheetName, backgroundColor).LastRow++;
            Sheet(sheetName, backgroundColor).LastColumn = 1;
        }

        public void Write(string sheetName, params object[] content)
        {
            Write(null, sheetName, content);
        }

        public void Write(Color? backgroundColor, string sheetName, params object[] content)
        {
            var sheet = Sheet(GetSheetName(sheetName), backgroundColor);
            foreach (var value in content)
            {
                sheet.SetValue(value, backgroundColor);
                sheet.LastColumn++;
            }
        }

        public void WriteAndMerge(Color? backgroundColor, string sheetName, int mergeAmount, object content)
        {
            var sheet = Sheet(GetSheetName(sheetName), backgroundColor);
            sheet.SetValue(content, backgroundColor);

            var cell = sheet.ExcelWorksheet.Cells[sheet.LastRow, sheet.LastColumn, sheet.LastRow, sheet.LastColumn + mergeAmount];
            cell.Merge = true;
            sheet.LastColumn += mergeAmount + 1;
        }

        private double GetRenderedTextHeight(string text, ExcelFont font, double width)
        {
            using (var bm = new Bitmap(1, 1))
            using (var graphics = Graphics.FromImage(bm))
            {
                var pixelWidth = Convert.ToInt32(width * 7.5);
                using (var drawingFont = new Font(font.Name, font.Size))
                {
                    var size = graphics.MeasureString(text, drawingFont, pixelWidth);
                    return Math.Min(Convert.ToDouble(size.Height) * 72 / 96, 409) * 1.2d;
                }
            }
        }

        public byte[] GetContent()
        {
            foreach (var sheet in ExcelPackage.Workbook.Worksheets)
            {
                var cells = sheet.Cells[sheet.Dimension.Address];
                cells.AutoFitColumns(0, 100);
                cells.Style.WrapText = true;
                cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                var start = sheet.Dimension.Start;
                var end = sheet.Dimension.End;
                for (var row = start.Row; row <= end.Row; row++)
                {
                    var hasValue = false;
                    for (var col = start.Column; col <= end.Column && !hasValue; col++)
                    {
                        if (!string.IsNullOrEmpty(sheet.Cells[row, col].Text))
                            hasValue = true;
                    }

                    if (hasValue)
                    {
                        foreach (var cell in sheet.Cells[row, 1, row, end.Column])
                            cell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }
                }

                foreach (var mergeAddress in sheet.MergedCells)
                {
                    var mergedRange = sheet.Cells[mergeAddress];
                    if (mergedRange.Value == null)
                        continue;

                    var value = (mergedRange.Value as object[,])?[0, 0]?.ToString();
                    if (string.IsNullOrEmpty(value))
                        continue;

                    var mergedRangeWidth = 0.0d;
                    for (var col = mergedRange.Start.Column; col <= mergedRange.End.Column; col++)
                    {
                        mergedRangeWidth += sheet.Column(col).Width;
                    }

                    var renderedTextHeight = GetRenderedTextHeight(value, mergedRange.Style.Font, mergedRangeWidth);
                    var row = sheet.Row(mergedRange.Start.Row);
                    if (renderedTextHeight > row.Height)
                    {
                        row.Height = renderedTextHeight;
                    }
                }
            }

            return ExcelPackage.GetAsByteArray();
        }

        public void WriteLink(string sheetName, string text, string targetSheetName, Color? backgroundColor = null)
        {
            sheetName = GetSheetName(sheetName);
            targetSheetName = GetSheetName(targetSheetName);

            Sheet(sheetName, backgroundColor).SetLink(text, targetSheetName, backgroundColor);
            Sheet(sheetName, backgroundColor).LastColumn++;
        }

        public void SetSheetColor(string sheetName, Color color)
        {
            sheetName = GetSheetName(sheetName);
            Sheet(sheetName).ExcelWorksheet.TabColor = color;
        }
    }
}