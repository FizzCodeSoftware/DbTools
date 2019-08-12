namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System.Configuration;
    using System.IO;

    public class PatternMatchingTableCustomizerFromCsv : PatternMatchingTableCustomizer
    {
        public PatternMatchingTableCustomizerFromCsv(string fileName)
        {
            var path = ConfigurationManager.AppSettings["WorkingDirectory"];

            // default name <dbname>.DbTools.Patterns.csv
            if (!fileName.EndsWith(".csv"))
                fileName += ".DbTools.Patterns.csv";

            using (var reader = new StreamReader(Path.Combine(path, fileName)))
            {
                reader.ReadLine(); // skip header row
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
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

                    AddPattern(pattern, patternExcept, shouldSkip, category, backgroundColor);
                }
            }
        }
    }
}
