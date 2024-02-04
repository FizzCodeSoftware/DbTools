namespace FizzCode.DbTools.DataDefinitionDocumenter;

using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

public abstract partial class DocumenterWriterBase
{
    protected void AddTableHeader(bool hasCategories, string category, SqlTable table, string firstColumn = null)
    {
        var mergeAmount = !Context.DocumenterSettings.NoInternalDataTypes ? 12 : 11;
        mergeAmount += firstColumn == null ? 0 : 1;

        WriteColor(table.SchemaAndTableName, "Schema");
        WriteAndMerge(table.SchemaAndTableName, mergeAmount, table.SchemaAndTableName.Schema);
        WriteLine(table.SchemaAndTableName);

        WriteColor(table.SchemaAndTableName, "Table name");
        WriteAndMerge(table.SchemaAndTableName, mergeAmount, table.SchemaAndTableName.TableName);
        WriteLine(table.SchemaAndTableName);

        var tableDescription = table.Properties.OfType<SqlTableDescription>().FirstOrDefault();

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

        WriteLine(table.SchemaAndTableName, tableColumns.ToArray());
    }

    protected void AddColumnsToTableSheet(SqlColumn column, ColumnDocumentInfo columnDocumentInfo, string firstColumn = null)
    {
        var table = column.Table;
        var sqlType = column.Type;

        if (firstColumn != null)
            Write(table.SchemaAndTableName, firstColumn);

        if (!Context.DocumenterSettings.NoInternalDataTypes)
            Write(table.SchemaAndTableName, column.Name, sqlType.SqlTypeInfo.SqlDataType, sqlType.SqlTypeInfo.SqlDataType, sqlType.Length, sqlType.Scale, sqlType.IsNullable);
        else
            Write(table.SchemaAndTableName, column.Name, sqlType, sqlType.Length, sqlType.Scale, sqlType.IsNullable);

        if (columnDocumentInfo.IsPk)
            Write(table.SchemaAndTableName, true);
        else
            Write(table.SchemaAndTableName, "");

        if (columnDocumentInfo.Identity != null)
            Write(table.SchemaAndTableName, $"IDENTITY ({columnDocumentInfo.Identity.Seed.ToString("D", CultureInfo.InvariantCulture)}, {columnDocumentInfo.Identity.Increment.ToString("D", CultureInfo.InvariantCulture)})");
        else
            Write(table.SchemaAndTableName, "");

        if (columnDocumentInfo.DefaultValue != null)
            Write(table.SchemaAndTableName, columnDocumentInfo.DefaultValue);
        else
            Write(table.SchemaAndTableName, "");

        Write(table.SchemaAndTableName, columnDocumentInfo.Description.Trim());

        // "Foreign Key Name", "Referenced Table", "Link", "Referenced Column"
        var fkOnColumn = table.Properties.OfType<ForeignKey>().FirstOrDefault(fk => fk.ForeignKeyColumns.Any(fkc => fkc.ForeignKeyColumn.Name == column.Name));

        if (fkOnColumn != null)
        {
            Write(table.SchemaAndTableName, fkOnColumn.Name);
            Write(table.SchemaAndTableName,
                Helper.GetSimplifiedSchemaAndTableName(fkOnColumn.ReferredTable.SchemaAndTableName));
            WriteLink(table.SchemaAndTableName, "link", fkOnColumn.ReferredTable.SchemaAndTableName);
            Write(table.SchemaAndTableName, fkOnColumn.ForeignKeyColumns.First(fkc => fkc.ForeignKeyColumn.Name == column.Name).ReferredColumn.Name);
        }

        WriteLine(table.SchemaAndTableName);
    }
}
