namespace FizzCode.DbTools.DataDefinitionDocumenter;

using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition;
using System.Linq;
using System.Collections.Generic;

public abstract partial class DocumenterWriterBase
{
    protected void AddTableHeader(bool hasCategories, string category, SqlTableOrView table, string firstColumn = null)
    {
        var mergeAmount = !Context.DocumenterSettings.NoInternalDataTypes ? 12 : 11;
        mergeAmount += firstColumn == null ? 0 : 1;

        WriteColor(table.SchemaAndTableName, "Schema");
        WriteAndMerge(table.SchemaAndTableName, mergeAmount, table.SchemaAndTableName.Schema);
        WriteLine(table.SchemaAndTableName);

        WriteColor(table.SchemaAndTableName, "Table name");
        WriteAndMerge(table.SchemaAndTableName, mergeAmount, table.SchemaAndTableName.TableName);
        WriteLine(table.SchemaAndTableName);

        var tableDescription = table switch
        {
            SqlTable sqlTable => sqlTable.Properties.OfType<SqlTableDescription>().FirstOrDefault(),
            SqlView sqlView => sqlView.Properties.OfType<SqlTableDescription>().FirstOrDefault(),
            _ => throw new System.ArgumentException("Unknown SqlTableOrView Type.")
        };

        WriteColor(table.SchemaAndTableName, "Description");
        WriteAndMerge(table.SchemaAndTableName, mergeAmount, tableDescription?.Description);
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

         var tableColumns = new List<string>();

        tableColumns.AddRange(new[] { "Column Name", "Data Type (DbTools)", "Data Type", "Column Length", "Column Scale", "Allow Nulls", "Primary Key", "Identity", "Default Value", "Description", "Foreign Key Name", "Referenced Table", "Link", "Referenced Column" });

        if (Context.DocumenterSettings.NoInternalDataTypes)
            tableColumns.Remove("Data Type (DbTools)");

        WriteLine(table.SchemaAndTableName, tableColumns);
    }
}
