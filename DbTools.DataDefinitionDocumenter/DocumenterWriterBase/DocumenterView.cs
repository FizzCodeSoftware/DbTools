namespace FizzCode.DbTools.DataDefinitionDocumenter;

using FizzCode.DbTools.DataDefinition.Base;
using System.Collections.Generic;

public abstract partial class DocumenterWriterBase
{
    protected void AddViewHeader(bool hasCategories, string category, SqlTableOrView table, string firstColumn = null)
    {
        var mergeAmount = !Context.DocumenterSettings.NoInternalDataTypes ? 12 : 11;
        mergeAmount += firstColumn == null ? 0 : 1;

        WriteColor(table.SchemaAndTableName, "Schema");
        WriteAndMerge(table.SchemaAndTableName, mergeAmount, table.SchemaAndTableName.Schema);
        WriteLine(table.SchemaAndTableName);

        WriteColor(table.SchemaAndTableName, "View name");
        WriteAndMerge(table.SchemaAndTableName, mergeAmount, table.SchemaAndTableName.TableName);
        WriteLine(table.SchemaAndTableName);

        if (hasCategories && !string.IsNullOrEmpty(category))
        {
            WriteColor(table.SchemaAndTableName, "Category");
            WriteAndMerge(table.SchemaAndTableName, mergeAmount, category);
            WriteLine(table.SchemaAndTableName);
        }

        WriteLine(table.SchemaAndTableName);
        if (firstColumn != null)
            Write(table.SchemaAndTableName, firstColumn);

        var viewColums = new List<string>();

        viewColums.AddRange(new[] { "Column Name", "Data Type (DbTools)", "Data Type", "Column Length", "Column Scale" });

        if (Context.DocumenterSettings.NoInternalDataTypes)
            viewColums.Remove("Data Type (DbTools)");

        WriteLine(table.SchemaAndTableName, viewColums.ToArray());
    }

    protected void AddColumnsToViewSheet(SqlViewColumn column, string firstColumn = null)
    {
        var table = column.SqlTableOrView;
        var sqlType = column.Type;

        if (firstColumn != null)
            Write(table.SchemaAndTableName, firstColumn);

        if (!Context.DocumenterSettings.NoInternalDataTypes)
            Write(table.SchemaAndTableName, column.Name, sqlType.SqlTypeInfo.SqlDataType, sqlType.SqlTypeInfo.SqlDataType, sqlType.Length, sqlType.Scale, sqlType.IsNullable);
        else
            Write(table.SchemaAndTableName, column.Name, sqlType, sqlType.Length, sqlType.Scale, sqlType.IsNullable);

        WriteLine(table.SchemaAndTableName);
    }
}
