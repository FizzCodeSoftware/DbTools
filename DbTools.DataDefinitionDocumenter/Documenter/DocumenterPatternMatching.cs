namespace FizzCode.DbTools.DataDefinitionDocumenter;

public partial class Documenter : DocumenterWriterBase
{
    private void AddPatternMatching()
    {
        if (Customizer is not PatternMatchingTableCustomizerWithTablesAndItems)
            return;

        var customizer = (PatternMatchingTableCustomizerWithTablesAndItems)Customizer;

        WriteLine("Patt.ma.-tables", "Table schema", "Table name", "Pattern", "Pattern except", "Should skip if match", "Category if match", "Background color if match");
        foreach (var tableAndPattern in customizer.TableMatches)
        {
            var schemaAndTableName = tableAndPattern.Key;
            var items = tableAndPattern.Value;

            foreach (var item in items)
            {
                WriteLine("Patt.ma.-tables", schemaAndTableName.Schema, schemaAndTableName.TableName, item.Pattern?.ToString(), item.PatternExcept?.ToString(), item.ShouldSkipIfMatch, item.CategoryIfMatch, item.BackGroundColorIfMatch);
            }
        }

        WriteLine("Patt.ma.-patterns", "Pattern", "Pattern except", "Table schema", "Table name", "Should skip if match", "Category if match", "Background color if match");
        foreach (var patternAndTable in customizer.MatchTables)
        {
            var item = patternAndTable.Key;
            var schemaAndTableNames = patternAndTable.Value;

            foreach (var schemaAndTableName in schemaAndTableNames)
            {
                WriteLine("Patt.ma.-patterns", item.Pattern?.ToString(), item.PatternExcept?.ToString(), schemaAndTableName.Schema, schemaAndTableName.TableName, item.ShouldSkipIfMatch, item.CategoryIfMatch, item.BackGroundColorIfMatch);
            }
        }

        WriteLine("Patt.ma.-ma.s w exceptions", "Pattern", "Pattern except", "Table schema", "Table name", "Should skip if match", "Category if match", "Background color if match");
        foreach (var patternAndTable in customizer.MatchTablesWithException)
        {
            var item = patternAndTable.Key;
            var schemaAndTableNames = patternAndTable.Value;

            foreach (var schemaAndTableName in schemaAndTableNames)
            {
                WriteLine("Patt.ma.-ma.s w exceptions", item.Pattern?.ToString(), item.PatternExcept?.ToString(), schemaAndTableName.Schema, schemaAndTableName.TableName, item.ShouldSkipIfMatch, item.CategoryIfMatch, item.BackGroundColorIfMatch);
            }
        }
    }

    private void AddPatternMatchingNoMatch()
    {
        if (Customizer is not PatternMatchingTableCustomizerWithTablesAndItems)
            return;

        var customizer = (PatternMatchingTableCustomizerWithTablesAndItems)Customizer;

        WriteLine("Patt.ma.-no matches (unused)", "Pattern", "Pattern except", "Should skip if match", "Category if match", "Background color if match");

        foreach (var item in customizer.PatternMatchingTableCustomizer.Patterns)
        {
            if (customizer.MatchTables.ContainsKey(item))
                continue;

            if (customizer.MatchTablesWithException.ContainsKey(item))
                continue;

            WriteLine("Patt.ma.-no matches (unused)", item.Pattern?.ToString(), item.PatternExcept?.ToString(), item.ShouldSkipIfMatch, item.CategoryIfMatch, item.BackGroundColorIfMatch);
        }
    }
}
