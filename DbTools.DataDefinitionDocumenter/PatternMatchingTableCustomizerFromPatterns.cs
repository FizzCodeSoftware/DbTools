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
            if (!fileName.EndsWith(".csv"))
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

        private static void ProcessPatternContentLine(PatternMatchingTableCustomizer customizer, string line)
        {
            var values = line.Split(';');

            var pattern = values[0];
            var patternExcept = values[1];
            var shouldSkip = values[2] != "0";
            string category = null;
            string backgroundColor = null;

            if (values.Length >= 4)
                category = values[3];

            if (values.Length >= 5)
                backgroundColor = values[4];

            customizer.AddPattern(pattern, patternExcept, shouldSkip, category, backgroundColor);
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
