namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System;
    using System.IO;

    public static class PatternMatchingTableCustomizerFromPatterns
    {
        public static PatternMatchingTableCustomizer FromCsv(string fileName, DocumenterSettings documenterSettings)
        {
            var customizer = new PatternMatchingTableCustomizer();

            var path = documenterSettings?.WorkingDirectory ?? "";

            // default name <dbname>.DbTools.Patterns.csv
            if (!fileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                fileName += ".DbTools.Patterns.csv";

            fileName = Path.Combine(path, fileName);

            if (!File.Exists(fileName))
                return null;

            using (var reader = new StreamReader(fileName))
            {
                reader.ReadLine(); // skip header row
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    ProcessPatternContentLine(customizer, line);
                }
            }

            return customizer;
        }

        private static string EmptyStringAsNull(string value)
        {
            return value.Length == 0 ? null : value;
        }

        private static void ProcessPatternContentLine(PatternMatchingTableCustomizer customizer, string line)
        {
            var values = line.Split(';');

            var patternSchema = EmptyStringAsNull(values[0]);
            var patternTableName = EmptyStringAsNull(values[1]);
            var patternExceptSchema = EmptyStringAsNull(values[2]);
            var patternExceptTableName = EmptyStringAsNull(values[3]);
            var shouldSkip = values[4] != "0";
            string category = null;
            string backgroundColor = null;

            if (values.Length >= 6)
                category = values[5];

            if (values.Length >= 7)
                backgroundColor = values[6];

            customizer.AddPattern(patternSchema, patternTableName, patternExceptSchema, patternExceptTableName, shouldSkip, category, backgroundColor);
        }

        public static PatternMatchingTableCustomizer FromString(string patternMatchingContent)
        {
            var customizer = new PatternMatchingTableCustomizer();

            foreach (var patternContentLine in patternMatchingContent.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                ProcessPatternContentLine(customizer, patternContentLine);

            return customizer;
        }
    }
}
