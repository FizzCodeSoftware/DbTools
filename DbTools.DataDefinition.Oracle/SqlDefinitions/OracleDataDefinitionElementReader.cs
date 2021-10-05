namespace FizzCode.DbTools.DataDefinitionReader
{
    using System.Collections.Generic;
    using FizzCode.DbTools.DataDefinition.SqlExecuter;

    public abstract class OracleDataDefinitionElementReader : GenericDataDefinitionElementReader
    {
        protected OracleDataDefinitionElementReader(SqlStatementExecuter executer, SchemaNamesToRead schemaNames)
            : base(executer, schemaNames)
        {
        }

        protected override void AddSchemaNamesFilter(ref string sqlStatement, string schemaColumnName)
        {
            var schemaNames = new List<string>();
            if (SchemaNames?.AllDefault != false)
            {
                if (Executer.Generator.Context.Settings.Options.ShouldUseDefaultSchema)
                    schemaNames.Add(Executer.Generator.Context.Settings.SqlVersionSpecificSettings.GetAs<string>("DefaultSchema"));
            }
            else
            {
                if (SchemaNames.AllNotSystem)
                {
                    sqlStatement += @"
AND EXISTS (SELECT 1 FROM dba_objects o
	WHERE o.owner = u.username ) AND u.default_tablespace not in
	('SYSTEM','SYSAUX') and u.ACCOUNT_STATUS = 'OPEN'";
                }

                if (!SchemaNames.All && SchemaNames.SchemaNames != null)
                {
                    schemaNames = SchemaNames.SchemaNames;
                }
            }

            if (schemaNames.Count > 0)
                sqlStatement += $" AND {schemaColumnName} IN({string.Join(',', schemaNames.ConvertAll(s => "'" + s + "'"))})";
        }
    }
}