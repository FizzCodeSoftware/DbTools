using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using FizzCode.DbTools.Common;

namespace FizzCode.DbTools.DataDefinitionDocumenter;
public abstract partial class DocumenterWriterBase
{
    protected void AddTableHeader(bool hasCategories, string? category, SqlTable table, string? firstColumn = null)
    {
        var mergeAmount = !Context.DocumenterSettings.NoInternalDataTypes ? 12 : 11;
        mergeAmount += firstColumn == null ? 0 : 1;

        var schemaAndTableName = table.SchemaAndTableNameSafe;
        WriteColor(schemaAndTableName, "Schema");
        WriteAndMerge(schemaAndTableName, mergeAmount, schemaAndTableName.Schema);
        WriteLine(schemaAndTableName);

        WriteColor(schemaAndTableName, "Table name");
        WriteAndMerge(schemaAndTableName, mergeAmount, schemaAndTableName.TableName);
        WriteLine(schemaAndTableName);

        var tableDescription = table.Properties.OfType<SqlTableDescription>().FirstOrDefault();

        WriteColor(schemaAndTableName, "Description");
        WriteAndMerge(schemaAndTableName, mergeAmount, tableDescription?.Description);
        WriteLine(schemaAndTableName);

        if (hasCategories && !string.IsNullOrEmpty(category))
        {
            WriteColor(schemaAndTableName, "Category");
            WriteAndMerge(schemaAndTableName, mergeAmount, category);
            WriteLine(schemaAndTableName);
        }

        WriteLine(schemaAndTableName);
        if (firstColumn != null)
            Write(schemaAndTableName, firstColumn);

        var tableColumns = new List<string>();

        tableColumns.AddRange(tableColumnNames);

        if (Context.DocumenterSettings.NoInternalDataTypes)
            tableColumns.Remove("Data Type (DbTools)");

        WriteLine(schemaAndTableName, tableColumns.ToArray());
    }

    private static readonly string[] tableColumnNames = ["Column Name", "Data Type (DbTools)", "Data Type", "Column Length", "Column Scale", "Allow Nulls", "Primary Key", "Identity", "Default Value", "Description", "Foreign Key Name", "Referenced Table", "Link", "Referenced Column"];

    protected void AddColumnsToTableSheet(SqlColumn column, ColumnDocumentInfo columnDocumentInfo, string? firstColumn = null)
    {
        var table = column.Table; 
        var sqlType = Throw.IfNull(column.Type);

        if (firstColumn != null)
            Write(table.SchemaAndTableName!, firstColumn);

        if (!Context.DocumenterSettings.NoInternalDataTypes)
            Write(table.SchemaAndTableName!, column.Name, sqlType.SqlTypeInfo.SqlDataType, sqlType.SqlTypeInfo.SqlDataType, sqlType.Length, sqlType.Scale, sqlType.IsNullable);
        else
            Write(table.SchemaAndTableName!, column.Name, sqlType, sqlType.Length, sqlType.Scale, sqlType.IsNullable);

        if (columnDocumentInfo.IsPk)
            Write(table.SchemaAndTableName!, true);
        else
            Write(table.SchemaAndTableName!, "");

        if (columnDocumentInfo.Identity != null)
            Write(table.SchemaAndTableName!, $"IDENTITY ({columnDocumentInfo.Identity.Seed.ToString("D", CultureInfo.InvariantCulture)}, {columnDocumentInfo.Identity.Increment.ToString("D", CultureInfo.InvariantCulture)})");
        else
            Write(table.SchemaAndTableName!, "");

        if (columnDocumentInfo.DefaultValue != null)
            Write(table.SchemaAndTableName!, columnDocumentInfo.DefaultValue);
        else
            Write(table.SchemaAndTableName!, "");

        Write(table.SchemaAndTableName!, columnDocumentInfo.Description?.Trim());

        // "Foreign Key Name", "Referenced Table", "Link", "Referenced Column"
        var fkOnColumn = table.Properties.OfType<ForeignKey>().FirstOrDefault(fk => fk.ForeignKeyColumns.Any(fkc => fkc.ForeignKeyColumn.Name == column.Name));

        if (fkOnColumn != null)
        {
            Write(table.SchemaAndTableName!, fkOnColumn.Name);
            Write(table.SchemaAndTableName!,
                Helper.GetSimplifiedSchemaAndTableName(fkOnColumn.ReferredTable));
            WriteLink(table.SchemaAndTableName!, "link", fkOnColumn.ReferredTable!.SchemaAndTableName!);
            Write(table.SchemaAndTableName!, fkOnColumn.ForeignKeyColumns.First(fkc => fkc.ForeignKeyColumn.Name == column.Name).ReferredColumn.Name);
        }

        WriteLine(table.SchemaAndTableName!);
    }
}
