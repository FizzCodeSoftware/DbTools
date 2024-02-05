using FizzCode.DbTools.DataDefinition.Base;
using System.Collections.Generic;

namespace FizzCode.DbTools.DataDefinitionDocumenter;
public abstract partial class DocumenterWriterBase
{
    protected void AddViewHeader(bool hasCategories, string category, SqlView view, string? firstColumn = null)
    {
        var mergeAmount = !Context.DocumenterSettings.NoInternalDataTypes ? 4 : 3;
        mergeAmount += firstColumn == null ? 0 : 1;

        var schemaAndTableName = view.SchemaAndTableNameSafe;
        WriteColor(schemaAndTableName, "Schema");
        WriteAndMerge(schemaAndTableName, mergeAmount, schemaAndTableName.Schema);
        WriteLine(schemaAndTableName);

        WriteColor(schemaAndTableName, "View name");
        WriteAndMerge(schemaAndTableName, mergeAmount, schemaAndTableName.TableName);
        WriteLine(schemaAndTableName);

        /*var viewDescription = view.Properties.OfType<SqlViewDescription>().FirstOrDefault();

        WriteColor(schemaAndTableName, "Description");
        WriteAndMerge(schemaAndTableName, mergeAmount, viewDescription?.Description);
        WriteLine(schemaAndTableName);*/

        if (hasCategories && !string.IsNullOrEmpty(category))
        {
            WriteColor(schemaAndTableName, "Category");
            WriteAndMerge(schemaAndTableName, mergeAmount, category);
            WriteLine(schemaAndTableName);
        }

        WriteLine(schemaAndTableName);
        if (firstColumn != null)
            Write(schemaAndTableName, firstColumn);

        var viewColums = new List<string>();

        viewColums.AddRange(new[] { "Column Name", "Data Type (DbTools)", "Data Type", "Column Length", "Column Scale" });

        if (Context.DocumenterSettings.NoInternalDataTypes)
            viewColums.Remove("Data Type (DbTools)");

        WriteLine(schemaAndTableName, viewColums.ToArray());
    }

    protected void AddColumnsToViewSheet(SqlViewColumn column, string? firstColumn = null)
    {
        var view = column.SqlTableOrView;
        var sqlType = column.Type;

        if (firstColumn != null)
            Write(view.SchemaAndTableName, firstColumn);

        if (!Context.DocumenterSettings.NoInternalDataTypes)
            Write(view.SchemaAndTableName, column.Name, sqlType.SqlTypeInfo.SqlDataType, sqlType.SqlTypeInfo.SqlDataType, sqlType.Length, sqlType.Scale, sqlType.IsNullable);
        else
            Write(view.SchemaAndTableName, column.Name, sqlType, sqlType.Length, sqlType.Scale, sqlType.IsNullable);

        WriteLine(view.SchemaAndTableName);
    }
}
