namespace FizzCode.DbTools.DataDefinitionReader
{
    using System.Collections.Generic;
    using FizzCode.DbTools.Common.Logger;
    using FizzCode.DbTools.SqlExecuter;

    public abstract class GenericDataDefinitionElementReader
    {
        protected GenericDataDefinitionElementReader(SqlStatementExecuter executer, SchemaNamesToRead schemaNames)
        {
            Executer = executer;
            SchemaNames = schemaNames;
        }

        protected SchemaNamesToRead SchemaNames { get; }

        protected SqlStatementExecuter Executer { get; }

        protected Logger Logger => Executer.Generator.Context.Logger;

        protected virtual void AddSchemaNamesFilter(ref string sqlStatement, string schemaColumnName)
        {
            var schemaNames = new List<string>();
            if (SchemaNames?.AllDefault != false)
            {
                if (Executer.Generator.Context.Settings.Options.ShouldUseDefaultSchema)
                    schemaNames.Add(Executer.Generator.Context.Settings.SqlVersionSpecificSettings.GetAs<string>("DefaultSchema"));
            }
            else if (!SchemaNames.All && SchemaNames.SchemaNames != null)
            {
                schemaNames = SchemaNames.SchemaNames;
            }

            if (schemaNames.Count > 0)
                sqlStatement += $" AND {schemaColumnName} IN({string.Join(',', schemaNames.ConvertAll(s => "'" + s + "'"))})";
        }
    }
}
