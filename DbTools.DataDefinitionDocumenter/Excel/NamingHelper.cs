namespace FizzCode.DbTools.DataDefinitionDocumenter.Excel
{
    using System;

    public static class NamingHelper
    {
        private static readonly UniqueName _uniqueName = new UniqueName();

        public static string GetUniqueName(string originalName)
        {
            return _uniqueName.GetUniqueName(originalName);
        }

        private static readonly char[] SpecialSheetNameCharacters = "()$,;-{}\"'（）【】“”‘’%…".ToCharArray();

        public static string QuoteSheetNameIfNeedeed(string sheetName)
        {
            if (sheetName.IndexOfAny(SpecialSheetNameCharacters) >= 0)
                return $"'{sheetName.Replace("'", "''", StringComparison.InvariantCulture)}'";
            else
                return sheetName;
        }

    }
}
