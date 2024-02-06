using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using FizzCode.DbTools.DataDefinition;
using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDefinitionDocumenter;
public abstract partial class DocumenterWriterBase : DocumenterBase
{
    protected new DocumenterContext Context => (DocumenterContext)base.Context;

    protected DocumenterWriterBase(IDocumenterWriter documenterWriter, DocumenterContext context, SqlEngineVersion version, string databaseName = "", string? fileName = null)
        : base(context, version, databaseName)
    {
        DocumenterWriter = documenterWriter;
        Helper = new DocumenterHelper(context.Settings);
        FileName = fileName;
    }

    protected IDocumenterWriter DocumenterWriter { get; set; }

    protected string? FileName { get; }

    public static bool ShouldSkipKnownTechnicalTable(SchemaAndTableName schemaAndTableName)
    {
        // TODO MS Sql specific
        // TODO Move
        // TODO Options
        return schemaAndTableName.SchemaAndName == "dbo.__RefactorLog"
            || schemaAndTableName.SchemaAndName == "dbo.sysdiagrams"
            || schemaAndTableName.SchemaAndName == "dbo._sys_blocks";
    }

    protected void Write(string sheetName, params object?[] content)
    {
        DocumenterWriter.Write(sheetName, FormatBoolContent(content));
    }

    protected void WriteLine(string sheetName, params object?[] content)
    {
        DocumenterWriter.WriteLine(sheetName, FormatBoolContent(content));
    }

    protected void Write(SchemaAndTableName schemaAndTableName, params object?[] content)
    {
        DocumenterWriter.Write(Helper.GetSimplifiedSchemaAndTableName(schemaAndTableName), FormatBoolContent(content));
    }

    protected void WriteColor(SchemaAndTableName schemaAndTableName, params object?[] content)
    {
        DocumenterWriter.Write(GetColor(schemaAndTableName), Helper.GetSimplifiedSchemaAndTableName(schemaAndTableName), content);
    }

    protected void WriteLine(SchemaAndTableName schemaAndTableName, params object?[] content)
    {
        DocumenterWriter.WriteLine(Helper.GetSimplifiedSchemaAndTableName(schemaAndTableName), FormatBoolContent(content));
    }

    protected void WriteLink(SchemaAndTableName schemaAndTableName, string text, SchemaAndTableName targetSchemaAndTableName, Color? backgroundColor = null)
    {
        DocumenterWriter.WriteLink(Helper.GetSimplifiedSchemaAndTableName(schemaAndTableName), text, Helper.GetSimplifiedSchemaAndTableName(targetSchemaAndTableName), backgroundColor);
    }

    protected void WriteAndMerge(SchemaAndTableName schemaAndTableName, int mergeAmount, params object?[] content)
    {
        DocumenterWriter.WriteAndMerge(GetColor(schemaAndTableName), Helper.GetSimplifiedSchemaAndTableName(schemaAndTableName), mergeAmount, FormatBoolContent(content));
    }

    protected void MergeUpFromPreviousRow(SchemaAndTableName schemaAndTableName, int mergeAmount)
    {
        DocumenterWriter.MergeUpFromPreviousRow(Helper.GetSimplifiedSchemaAndTableName(schemaAndTableName), mergeAmount);
    }

    protected virtual Color? GetColor(SchemaAndTableName schemaAndTableName)
    {
        // TODO coloring to incude schema
        var hexColor = Context.Customizer.BackGroundColor(schemaAndTableName);

        if (hexColor is null)
            return null;

        return ColorTranslator.FromHtml(hexColor);
    }

    protected static ColumnDocumentInfo GetColumnDocumentInfo(List<PrimaryKey> pks, SqlColumn column)
    {
        var info = new ColumnDocumentInfo();

        var descriptionProperty = column.Properties.OfType<SqlColumnDescription>().FirstOrDefault();
        info.Description = "";
        if (descriptionProperty != null)
            info.Description = descriptionProperty.Description;

        info.IsPk = pks.Any(pk => pk.SqlColumns.Any(cao => cao.SqlColumn == column));
        info.Identity = column.Properties.OfType<Identity>().FirstOrDefault();
        info.DefaultValue = column.Properties.OfType<DefaultValue>().FirstOrDefault();

        return info;
    }

    protected static List<SqlTable> RemoveKnownTechnicalTables(List<SqlTable> list)
    {
        return list.Where(x => !ShouldSkipKnownTechnicalTable(x.SchemaAndTableName!)).ToList();
    }

    private static object?[] FormatBoolContent(params object?[] content)
    {
        var result = new List<object?>();
        foreach (var obj in content)
        {
            if (obj is bool objAsBool)
                result.Add(objAsBool ? "TRUE" : "FALSE");
            else
                result.Add(obj);
        }

        return result.ToArray();
    }

    protected class ColumnDocumentInfo
    {
        public string? Description { get; set; }
        public bool IsPk { get; set; }
        public Identity? Identity { get; set; }
        public DefaultValue? DefaultValue { get; set; }
    }
}
