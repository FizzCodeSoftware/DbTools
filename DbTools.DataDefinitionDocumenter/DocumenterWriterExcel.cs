namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using OfficeOpenXml;
    using OfficeOpenXml.Style;

    public class DocumenterWriterExcel : IDocumenterWriter
    {
        private ExcelPackage ExcelPackage { get; }
        private readonly UniqueName _uniqueName;

        public DocumenterWriterExcel()
        {
            ExcelPackage = new ExcelPackage();
            _uniqueName = new UniqueName();
        }

        private readonly Dictionary<string, Sheet> _sheets = new Dictionary<string, Sheet>();

        protected string GetSheetName(string name)
        {
            return _uniqueName.GetUniqueName(name);
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
            Write(sheetName, content);

            Sheet(sheetName).LastRow++;
            Sheet(sheetName).LastColumn = 1;
        }

        public void WriteLine(Color? backgroundColor, string sheetName, params object[] content)
        {
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
            var sheet = Sheet(sheetName, backgroundColor);
            foreach (var value in content)
            {
                sheet.SetValue(value, backgroundColor);
                sheet.LastColumn++;
            }
        }

        public void WriteAndMerge(Color? backgroundColor, string sheetName, int mergeAmount, object content)
        {
            var sheet = Sheet(sheetName, backgroundColor);
            sheet.SetValue(content, backgroundColor);

            var cell = sheet.ExcelWorksheet.Cells[sheet.LastRow, sheet.LastColumn, sheet.LastRow, sheet.LastColumn + mergeAmount];
            cell.Merge = true;
            sheet.LastColumn += mergeAmount + 1;
        }

        public void MergeUpFromPreviousRow(string sheetName, int mergeAmount)
        {
            var sheet = Sheet(sheetName);
            var cell = sheet.ExcelWorksheet.Cells[sheet.LastRow - mergeAmount - 1, sheet.LastColumn, sheet.LastRow - 1, sheet.LastColumn];
            cell.Merge = true;
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
                cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

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
                            cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
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
            targetSheetName = GetSheetName(targetSheetName);

            Sheet(sheetName, backgroundColor).SetLink(text, targetSheetName, backgroundColor);
            Sheet(sheetName, backgroundColor).LastColumn++;
        }

        public void SetSheetColor(string sheetName, Color color)
        {
            Sheet(sheetName).ExcelWorksheet.TabColor = color;
        }
    }
}