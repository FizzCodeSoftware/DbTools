namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using FizzCode.DbTools.Common.Logger;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;

    public abstract class DocumenterWriterBase : DocumenterBase
    {
        protected DocumenterWriterBase(IDocumenterWriter documenterWriter, DocumenterContext context, SqlEngineVersion version, string databaseName = "", string fileName = null)
            : base(context, version, databaseName)
        {
            DocumenterWriter = documenterWriter;
            Helper = new DocumenterHelper(context.Settings);
            FileName = fileName;
        }

        protected IDocumenterWriter DocumenterWriter { get; set; }

        protected string FileName { get; }

        public static bool ShouldSkipKnownTechnicalTable(SchemaAndTableName schemaAndTableName)
        {
            // TODO MS Sql specific
            // TODO Move
            // TODO Options
            return schemaAndTableName.SchemaAndName == "dbo.__RefactorLog"
                || schemaAndTableName.SchemaAndName == "dbo.sysdiagrams";
        }

        protected void Write(string sheetName, params object[] content)
        {
            DocumenterWriter.Write(sheetName, content);
        }

        protected void WriteLine(string sheetName, params object[] content)
        {
            DocumenterWriter.WriteLine(sheetName, content);
        }

        protected void Write(SchemaAndTableName schemaAndTableName, params object[] content)
        {
            DocumenterWriter.Write(Helper.GetSimplifiedSchemaAndTableName(schemaAndTableName), content);
        }

        protected void WriteColor(SchemaAndTableName schemaAndTableName, params object[] content)
        {
            DocumenterWriter.Write(GetColor(schemaAndTableName), Helper.GetSimplifiedSchemaAndTableName(schemaAndTableName), content);
        }

        protected void WriteLine(SchemaAndTableName schemaAndTableName, params object[] content)
        {
            DocumenterWriter.WriteLine(Helper.GetSimplifiedSchemaAndTableName(schemaAndTableName), content);
        }

        protected void WriteLink(SchemaAndTableName schemaAndTableName, string text, SchemaAndTableName targetSchemaAndTableName, Color? backgroundColor = null)
        {
            DocumenterWriter.WriteLink(Helper.GetSimplifiedSchemaAndTableName(schemaAndTableName), text, Helper.GetSimplifiedSchemaAndTableName(targetSchemaAndTableName), backgroundColor);
        }

        protected void WriteAndMerge(SchemaAndTableName schemaAndTableName, int mergeAmount, params object[] content)
        {
            DocumenterWriter.WriteAndMerge(GetColor(schemaAndTableName), Helper.GetSimplifiedSchemaAndTableName(schemaAndTableName), mergeAmount, content);
        }

        protected Color? GetColor(SchemaAndTableName schemaAndTableName)
        {
            // TODO coloring to incude schema
            var hexColor = Context.Customizer.BackGroundColor(schemaAndTableName);

            if (hexColor == null)
                return null;

            return ColorTranslator.FromHtml(hexColor);
        }
    }
}
