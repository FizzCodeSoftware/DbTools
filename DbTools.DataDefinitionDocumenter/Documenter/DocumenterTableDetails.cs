using FizzCode.DbTools.Common;
using FizzCode.DbTools.Common.Logger;
using FizzCode.DbTools.DataDefinition.Base;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace FizzCode.DbTools.DataDefinitionDocumenter;
public partial class Documenter
{
    private void AddTables(List<KeyValuePair<string?, SqlTable>> _sqlTablesByCategory, List<KeyValuePair<string?, SqlTable>> _skippedSqlTablesByCategory, bool hasCategories)
    {
        foreach (var tableKvp in _sqlTablesByCategory.OrderBy(kvp => kvp.Key).ThenBy(t => t.Value.SchemaAndTableNameSafe.Schema).ThenBy(t => t.Value.SchemaAndTableNameSafe.TableName))
        {
            Context.Logger.Log(LogSeverity.Verbose, "Generating {TableName}.", "Documenter", tableKvp.Value.SchemaAndTableNameSafe);
            var category = tableKvp.Key;
            var table = tableKvp.Value;
            AddTableToTableList(category, table, hasCategories);

            var sheetColor = GetColor(table.SchemaAndTableNameSafe);
            if (sheetColor != null)
                DocumenterWriter.SetSheetColor(Helper.GetSimplifiedSchemaAndTableName(table.SchemaAndTableNameSafe), sheetColor.Value);

            AddTableHeader(hasCategories, category, table);

            AddTableDetails(category, table, hasCategories);
        }

        WriteLine("Tables");

        foreach (var tableKvp in _skippedSqlTablesByCategory.OrderBy(kvp => kvp.Key).ThenBy(t => t.Value.SchemaAndTableNameSafe.Schema).ThenBy(t => t.Value.SchemaAndTableNameSafe.TableName))
        {
            var category = tableKvp.Key;
            var table = tableKvp.Value;
            AddTableToTableList(category, table, hasCategories);
        }
    }

    protected void AddTableDetails(string? category, SqlTable table, bool hasCategories)
    {
        var pks = table.Properties.OfType<PrimaryKey>().ToList();

        foreach (var column in table.Columns)
        {
            // TODO Create ISqlTypeMapper according to SqlDialect
            Throw.InvalidOperationExceptionIfNull(column.Type);
            var sqlType = column.Type;

            var columnDocumentInfo = GetColumnDocumentInfo(pks, column);

            // TODO internal data types are not OK this way

            AddColumnsToTableSheet(column, columnDocumentInfo);

            var schemaAndTableName = table.SchemaAndTableNameSafe;

            if (hasCategories)
            {
                if (!Context.DocumenterSettings.NoInternalDataTypes)
                    DocumenterWriter.Write(GetColor(schemaAndTableName), "All columns", category, schemaAndTableName.Schema, schemaAndTableName.TableName, column.Name, sqlType.SqlTypeInfo.SqlDataType, sqlType.SqlTypeInfo.SqlDataType, sqlType.Length, sqlType.Scale, sqlType.IsNullable);
                else
                    DocumenterWriter.Write(GetColor(schemaAndTableName), "All columns", category, schemaAndTableName.Schema, schemaAndTableName.TableName, column.Name, sqlType, sqlType.Length, sqlType.Scale, sqlType.IsNullable);
            }
            else if (!Context.DocumenterSettings.NoInternalDataTypes)
            {
                DocumenterWriter.Write(GetColor(schemaAndTableName), "All columns", schemaAndTableName.Schema, schemaAndTableName.TableName, column.Name, sqlType.SqlTypeInfo.SqlDataType, sqlType.SqlTypeInfo.SqlDataType, sqlType.Length, sqlType.Scale, sqlType.IsNullable);
            }
            else
            {
                DocumenterWriter.Write(GetColor(schemaAndTableName), "All columns", schemaAndTableName.Schema, schemaAndTableName.TableName, column.Name, sqlType.SqlTypeInfo.SqlDataType, sqlType.Length, sqlType.Scale, sqlType.IsNullable);
            }

            if (columnDocumentInfo.IsPk)
                DocumenterWriter.Write(GetColor(schemaAndTableName), "All columns", true);
            else
                DocumenterWriter.Write(GetColor(schemaAndTableName), "All columns", "");

            if (columnDocumentInfo.Identity != null)
                DocumenterWriter.Write(GetColor(schemaAndTableName), "All columns", $"IDENTITY ({columnDocumentInfo.Identity.Seed.ToString("D", CultureInfo.InvariantCulture)}, {columnDocumentInfo.Identity.Increment.ToString("D", CultureInfo.InvariantCulture)})");
            else
                DocumenterWriter.Write(GetColor(schemaAndTableName), "All columns", "");

            DocumenterWriter.Write(GetColor(schemaAndTableName), "All columns", columnDocumentInfo.DefaultValue);
            DocumenterWriter.WriteLine(GetColor(schemaAndTableName), "All columns", columnDocumentInfo.Description);
        }

        AddForeignKeysToTableSheet(table);

        AdIndexesToTableSheet(table);

        AddUniqueConstraintsToTableSheet(table);
    }

    private void AddForeignKeysToTableSheet(SqlTable table)
    {
        var mergeAmount = 1 + (!Context.DocumenterSettings.NoInternalDataTypes ? 12 : 11);

        if (!Context.DocumenterSettings.NoForeignKeys)
        {
            Throw.InvalidOperationExceptionIfNull(table.SchemaAndTableName);
            WriteLine(table.SchemaAndTableName);

            WriteAndMerge(table.SchemaAndTableName, mergeAmount, "Foreign keys");
            WriteLine(table.SchemaAndTableName);

            var fks = table.Properties.OfType<ForeignKey>().ToList();

            if (fks.Count > 0)
            {
                // TODO allow nulls. Check / other properties?
                WriteLine(table.SchemaAndTableName, "Foreign key name", "Column", "Referenced Table", "link", "Referenced Column", "Properties");
            }

            foreach (var fk in fks)
            {
                AddForeignKey(fk);
            }

            if (fks.Count > 0)
                WriteLine(table.SchemaAndTableName);
        }
    }

    private void AdIndexesToTableSheet(SqlTable table)
    {
        Throw.InvalidOperationExceptionIfNull(table.SchemaAndTableName);
        if (!Context.DocumenterSettings.NoIndexes)
        {
            WriteLine(table.SchemaAndTableName);

            var mergeAmount = 1 + (!Context.DocumenterSettings.NoInternalDataTypes ? 12 : 11);

            WriteAndMerge(table.SchemaAndTableName, mergeAmount, "Indexes");
            WriteLine(table.SchemaAndTableName);

            var indexes = table.Properties.OfType<Index>().ToList();

            if (indexes.Count > 0)
                WriteLine(table.SchemaAndTableName, "Index name", "Column", "Order", "Include");

            foreach (var index in indexes)
            {
                AddIndex(index);
            }
        }
    }

    private void AddUniqueConstraintsToTableSheet(SqlTable table)
    {
        if (!Context.DocumenterSettings.NoUniqueConstraints)
        {
            Throw.InvalidOperationExceptionIfNull(table.SchemaAndTableName);
            WriteLine(table.SchemaAndTableName);

            var mergeAmount = 1 + (!Context.DocumenterSettings.NoInternalDataTypes ? 12 : 11);

            WriteAndMerge(table.SchemaAndTableName, mergeAmount, "Unique constraints");
            WriteLine(table.SchemaAndTableName);

            var uniqueConstraints = table.Properties.OfType<UniqueConstraint>().ToList();

            if (uniqueConstraints.Count > 0)
                WriteLine(table.SchemaAndTableName, "Unique constraint name", "Column");

            foreach (var uniqueConstraint in uniqueConstraints)
            {
                AddUniqueConstraint(uniqueConstraint);
            }
        }
    }
}
