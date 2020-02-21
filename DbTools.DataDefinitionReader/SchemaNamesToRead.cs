namespace FizzCode.DbTools.DataDefinitionReader
{
    using System.Collections.Generic;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Common.Logger;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.SqlExecuter;

    public class SchemaNamesToRead
    {
        public SchemaNamesToRead(List<string> schemaNames)
        {
            SchemaNames = schemaNames;
        }

        public SchemaNamesToRead(bool AllDefaultNotSystem = true, bool AllNotSytem = false, bool all = false)
        {
            AllDefault = AllDefaultNotSystem;
            All = all;
        }

        public bool All { get; set; }
        public bool AllNotSystem { get; set; }
        public bool AllDefault { get; set; }

        public List<string> SchemaNames { get; set; }

        public static implicit operator SchemaNamesToRead(List<string> schemaNames)
        {
            if(schemaNames.Count == 0)
                return new SchemaNamesToRead(true);

            return new SchemaNamesToRead(schemaNames);
        }

        public static SchemaNamesToRead AllSchemas => new SchemaNamesToRead(false, false, true);
        public static SchemaNamesToRead AllNotSystemSchemas => new SchemaNamesToRead(false, true);
        public static SchemaNamesToRead AllDefaultSchemas => new SchemaNamesToRead(true);
    }
}
