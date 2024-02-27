using System.Collections.Generic;
using FizzCode.DbTools.Common;
using FizzCode.DbTools.Common.Logger;
using FizzCode.DbTools.DataDefinition.Base.Interfaces;
using FizzCode.DbTools.SqlExecuter;

namespace FizzCode.DbTools.DataDefinitionReader;
public abstract class GenericDataDefinitionElementReader
{
    protected GenericDataDefinitionElementReader(SqlStatementExecuter executer, ISchemaNamesToRead? schemaNames)
    {
        Executer = executer;

        schemaNames ??= new SchemaNamesToRead(true);

        SchemaNames = schemaNames;
    }

    protected ISchemaNamesToRead SchemaNames { get; }

    protected SqlStatementExecuter Executer { get; }

    protected Logger Logger => Executer.Context.Logger;

    protected virtual void AddSchemaNamesFilter(ref string sqlStatement, string schemaColumnName)
    {
        var schemaNames = new List<string>();
        if (SchemaNames?.AllDefault != false)
        {
            if (Executer.Context.Settings.Options.ShouldUseDefaultSchema)
            {
                schemaNames.Add(
                    Throw.IfNull(Executer.Context.Settings.SqlVersionSpecificSettings.GetAs<string>("DefaultSchema"))
                );
            }
        }
        else if (!SchemaNames.All && SchemaNames.SchemaNames != null)
        {
            schemaNames = SchemaNames.SchemaNames;
        }

        if (schemaNames.Count > 0)
            sqlStatement += $" AND {schemaColumnName} IN({string.Join(',', schemaNames.ConvertAll(s => "'" + s + "'"))})";
    }
}
