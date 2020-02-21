namespace FizzCode.DbTools.DataDefinitionReader
{
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.Common.Logger;
    using FizzCode.DbTools.DataDefinition.SqlExecuter;

    public abstract class GenericDataDefinitionElementReader
    {
        protected GenericDataDefinitionElementReader(SqlStatementExecuter executer, List<string> schemaNames = null)
        {
            Executer = executer;
            if (schemaNames != null)
                SchemaNames = schemaNames;
        }

        protected List<string> SchemaNames { get; } = new List<string>();

        protected SqlStatementExecuter Executer { get; set; }

        protected Logger Logger => Executer.Generator.Context.Logger;

        protected void AddSchemaNamesFilter(ref string sqlStatement, string schemaColumnName)
        {
            var schemaNames = SchemaNames;
            if (Executer.Generator.Context.Settings.Options.ShouldUseDefaultSchema
                && !SchemaNames.Contains(Executer.Generator.Context.Settings.SqlVersionSpecificSettings.GetAs<string>("DefaultSchema")))
            {
                schemaNames.Add(Executer.Generator.Context.Settings.SqlVersionSpecificSettings.GetAs<string>("DefaultSchema"));
            }

            if (schemaNames != null)
            {
                sqlStatement += $" AND {schemaColumnName} IN({string.Join(',', schemaNames.Select(s => "'" + s + "'").ToList())})";
            }
        }
    }
}
