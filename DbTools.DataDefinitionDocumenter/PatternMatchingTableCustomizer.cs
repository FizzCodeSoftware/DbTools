namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using FizzCode.DbTools.DataDefinition;

    public class PatternMatchingTableCustomizer : ITableCustomizer
    {
        protected List<PatternMatchingTableCustomizerItem> Patterns { get; } = new List<PatternMatchingTableCustomizerItem>();

        public void AddPattern(string patternSchema, string patternTableName, string patternExceptSchema, string patternExceptTableName, bool shouldSkip, string category, string backGroundColor)
        {
            Patterns.Add(new PatternMatchingTableCustomizerItem(new SchemaAndTableName(patternSchema, patternTableName), new SchemaAndTableName(patternExceptSchema, patternExceptTableName), shouldSkip, category, backGroundColor));
        }

        public string BackGroundColor(SchemaAndTableName tableName)
        {
            var item = GetPatternMatching(tableName);
            return item?.BackGroundColorIfMatch;
        }

        public string Category(SchemaAndTableName schemaAndTableName)
        {
            var item = GetPatternMatching(schemaAndTableName);
            return item?.CategoryIfMatch;
        }

        public bool ShouldSkip(SchemaAndTableName schemaAndTableName)
        {
            var item = GetPatternMatching(schemaAndTableName);
            return item?.ShouldSkipIfMatch == true;
        }

        private PatternMatchingTableCustomizerItem GetPatternMatching(SchemaAndTableName schemaAndTableName)
        {
            PatternMatchingTableCustomizerItem matchingItem = null;
            foreach (var item in Patterns)
            {
                if (IsRegex(item.Pattern))
                {
                    var regexPatternSchema = RegexFormFromWildCharForm(item.Pattern.Schema);
                    if (regexPatternSchema != null)
                        regexPatternSchema = "^" + regexPatternSchema;

                    var regexPatternTableName = RegexFormFromWildCharForm(item.Pattern.TableName);
                    if (regexPatternTableName != null)
                        regexPatternTableName = "^" + regexPatternTableName;

                    if (CheckMatchRegex(schemaAndTableName, regexPatternSchema, regexPatternTableName)
                        && ShouldNotSkipPatternExcept(item, schemaAndTableName))
                    {
                        if (matchingItem == null)
                            matchingItem = item;
                        else
                            throw new ApplicationException($"Multiple patterns are matching for {schemaAndTableName.SchemaAndName}.");
                    }
                }
                else if (CheckMatchString(item.Pattern, schemaAndTableName)
                    && ShouldNotSkipPatternExcept(item, schemaAndTableName))
                {
                    matchingItem = item;
                    break;
                }
            }

            return matchingItem;
        }

        private static bool CheckMatchString(SchemaAndTableName schemaAndTableNamePattern, SchemaAndTableName schemaAndTableNameActual)
        {
            if (schemaAndTableNamePattern.Schema == null)
                return string.Equals(schemaAndTableNamePattern.TableName, schemaAndTableNameActual.TableName, StringComparison.InvariantCultureIgnoreCase);

            if (schemaAndTableNamePattern.TableName == null)
                return string.Equals(schemaAndTableNamePattern.Schema, schemaAndTableNameActual.Schema, StringComparison.InvariantCultureIgnoreCase);

            return schemaAndTableNamePattern == schemaAndTableNameActual;
        }

        private static bool CheckMatchRegex(SchemaAndTableName schemaAndTableName, string regexPatternSchema, string regexPatternTableName)
        {
            if (regexPatternSchema == null)
                return Regex.Match(schemaAndTableName.TableName, regexPatternTableName).Success;

            if (regexPatternTableName == null)
                return Regex.Match(schemaAndTableName.Schema, regexPatternSchema).Success;

            return Regex.Match(schemaAndTableName.TableName, regexPatternTableName).Success
                && Regex.Match(schemaAndTableName.Schema, regexPatternSchema).Success;
        }

        private static bool ShouldNotSkipPatternExcept(PatternMatchingTableCustomizerItem item, SchemaAndTableName schemaAndTableName)
        {
            if (IsRegex(item.PatternExcept))
            {
                var regexPatternExceptSchema = RegexFormFromWildCharForm(item.PatternExcept.Schema);
                var regexPatternExceptTableName = RegexFormFromWildCharForm(item.PatternExcept.TableName);

                return !CheckMatchRegex(schemaAndTableName, regexPatternExceptSchema, regexPatternExceptTableName);
            }

            if (item.PatternExcept.Schema == null && item.PatternExcept.TableName == null)
                return true;
            else
                return !CheckMatchString(item.PatternExcept, schemaAndTableName);
        }

        private static string RegexFormFromWildCharForm(string schemaOrTableName)
        {
            if (schemaOrTableName == null)
                return null;

            return Regex.Escape(schemaOrTableName).Replace(@"\*", ".*", StringComparison.OrdinalIgnoreCase).Replace(@"\?", ".", StringComparison.OrdinalIgnoreCase).Replace("#", @"\d", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsRegex(string pattern)
        {
            if (pattern == null)
                return false;

            return pattern.Contains("*", StringComparison.OrdinalIgnoreCase) || pattern.Contains("?", StringComparison.OrdinalIgnoreCase) || pattern.Contains("#", StringComparison.OrdinalIgnoreCase);
        }
    }
}
