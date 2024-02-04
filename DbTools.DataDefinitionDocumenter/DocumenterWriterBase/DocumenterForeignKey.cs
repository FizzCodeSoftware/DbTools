namespace FizzCode.DbTools.DataDefinitionDocumenter;

using FizzCode.DbTools.DataDefinition.Base;
using System.Linq;
using System.Text;

public abstract partial class DocumenterWriterBase
{
    protected void AddForeignKey(ForeignKey fk, string firstColumn = null)
    {
        var countToMerge = 0;
        var table = fk.SqlTable;

        foreach (var fkColumn in fk.ForeignKeyColumns)
        {
            if (firstColumn != null)
                Write(table.SchemaAndTableName, firstColumn);

            Write(table.SchemaAndTableName, fk.Name, fkColumn.ForeignKeyColumn.Name, Helper.GetSimplifiedSchemaAndTableName(fk.ReferredTable.SchemaAndTableName));
            WriteLink(table.SchemaAndTableName, "link", Helper.GetSimplifiedSchemaAndTableName(fk.ReferredTable.SchemaAndTableName), GetColor(fk.ReferredTable.SchemaAndTableName));
            Write(table.SchemaAndTableName, fkColumn.ReferredColumn.Name);

            if (fk.SqlEngineVersionSpecificProperties.Any())
            {
                var propertySb = new StringBuilder();
                foreach (var sqlEngineVersionSpecificProperty in fk.SqlEngineVersionSpecificProperties)
                {
                    propertySb.Append(sqlEngineVersionSpecificProperty.Version)
                        .Append('/')
                        .Append(sqlEngineVersionSpecificProperty.Name)
                        .Append(" = ")
                        .AppendLine(sqlEngineVersionSpecificProperty.Value);
                }

                WriteLine(table.SchemaAndTableName, propertySb.ToString());
            }
            else
            {
                WriteLine(table.SchemaAndTableName);
            }

            countToMerge++;
        }

        if (countToMerge > 1)
        {
            MergeUpFromPreviousRow(table.SchemaAndTableName, countToMerge - 1);
        }
    }
}
