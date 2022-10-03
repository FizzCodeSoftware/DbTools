namespace FizzCode.DbTools.DataDefinitionReader
{
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.DataDefinition.Base.Interfaces;

    public class SchemaNamesToRead : ISchemaNamesToRead
    {
        public SchemaNamesToRead(List<string> schemaNames)
        {
            SchemaNames = schemaNames;
        }

        public SchemaNamesToRead(bool allDefaultNotSystem = true, bool allNotSystem = false, bool all = false)
        {
            AllDefault = allDefaultNotSystem;
            All = all;
            AllNotSystem = allNotSystem;
        }

        public bool All { get; set; }
        public bool AllNotSystem { get; set; }
        public bool AllDefault { get; set; }

        public List<string> SchemaNames { get; set; }

        public static implicit operator SchemaNamesToRead(List<string> schemaNames)
        {
            if (schemaNames.Count == 0)
                return new SchemaNamesToRead(true);

            return new SchemaNamesToRead(schemaNames);
        }

        public static SchemaNamesToRead AllSchemas => new(false, false, true);
        public static SchemaNamesToRead AllNotSystemSchemas => new(false, true);
        public static SchemaNamesToRead AllDefaultSchemas => new(true);

        public static ISchemaNamesToRead ToSchemaNames(IEnumerable<string> schemaNames)
        {
            var schemaNamesList = schemaNames.ToList();
            if (schemaNamesList.Count == 0)
                return new SchemaNamesToRead(true);

            return new SchemaNamesToRead(schemaNamesList);
        }
    }
}
