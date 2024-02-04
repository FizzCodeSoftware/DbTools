namespace FizzCode.DbTools.DataDefinitionDocumenter;

using FizzCode.DbTools.DataDefinition.Base;

public abstract partial class DocumenterWriterBase
{
    protected void AddIndex(Index index, string firstColumn = null)
    {
        var countToMerge = 0;
        var table = index.SqlTable;

        foreach (var indexColumn in index.SqlColumns)
        {
            if (firstColumn != null)
                Write(table.SchemaAndTableName, firstColumn);

            Write(table.SchemaAndTableName, index.Name);
            Write(table.SchemaAndTableName, indexColumn.SqlColumn.Name);
            WriteLine(table.SchemaAndTableName, indexColumn.OrderAsKeyword);

            countToMerge++;
        }

        foreach (var includeColumn in index.Includes)
        {
            Write(table.SchemaAndTableName, index.Name, includeColumn.Name, "");
            WriteLine(table.SchemaAndTableName, "YES");

            countToMerge++;
        }

        if (countToMerge > 1)
        {
            MergeUpFromPreviousRow(table.SchemaAndTableName, countToMerge - 1);
        }
    }
}
