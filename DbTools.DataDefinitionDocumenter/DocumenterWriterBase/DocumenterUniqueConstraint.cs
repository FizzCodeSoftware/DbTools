using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDefinitionDocumenter;
public abstract partial class DocumenterWriterBase
{
    protected void AddUniqueConstraint(UniqueConstraint uniqueConstraint, string firstColumn = null)
    {
        var countToMerge = 0;
        var table = uniqueConstraint.SqlTableOrView;

        foreach (var indexColumn in uniqueConstraint.SqlColumns)
        {
            if (firstColumn != null)
                Write(table.SchemaAndTableName, firstColumn);

            Write(table.SchemaAndTableName, uniqueConstraint.Name);
            WriteLine(table.SchemaAndTableName, indexColumn.SqlColumn.Name);

            countToMerge++;
        }

        if (countToMerge > 1)
        {
            MergeUpFromPreviousRow(table.SchemaAndTableName, countToMerge - 1);
        }
    }
}
