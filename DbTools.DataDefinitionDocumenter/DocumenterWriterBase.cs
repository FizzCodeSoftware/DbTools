namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using FizzCode.DbTools.DataDefinition;

    public abstract class DocumenterWriterBase : DocumenterBase
    {
        protected new DocumenterContext Context => (DocumenterContext)base.Context;

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
                || schemaAndTableName.SchemaAndName == "dbo.sysdiagrams"
                || schemaAndTableName.SchemaAndName == "dbo._sys_blocks";
        }

        protected void Write(string sheetName, params object[] content)
        {
            DocumenterWriter.Write(sheetName, FormatBoolContent(content));
        }

        protected void WriteLine(string sheetName, params object[] content)
        {
            DocumenterWriter.WriteLine(sheetName, FormatBoolContent(content));
        }

        protected void Write(SchemaAndTableName schemaAndTableName, params object[] content)
        {
            DocumenterWriter.Write(Helper.GetSimplifiedSchemaAndTableName(schemaAndTableName), FormatBoolContent(content));
        }

        protected void WriteColor(SchemaAndTableName schemaAndTableName, params object[] content)
        {
            DocumenterWriter.Write(GetColor(schemaAndTableName), Helper.GetSimplifiedSchemaAndTableName(schemaAndTableName), content);
        }

        protected void WriteLine(SchemaAndTableName schemaAndTableName, params object[] content)
        {
            DocumenterWriter.WriteLine(Helper.GetSimplifiedSchemaAndTableName(schemaAndTableName), FormatBoolContent(content));
        }

        protected void WriteLink(SchemaAndTableName schemaAndTableName, string text, SchemaAndTableName targetSchemaAndTableName, Color? backgroundColor = null)
        {
            DocumenterWriter.WriteLink(Helper.GetSimplifiedSchemaAndTableName(schemaAndTableName), text, Helper.GetSimplifiedSchemaAndTableName(targetSchemaAndTableName), backgroundColor);
        }

        protected void WriteAndMerge(SchemaAndTableName schemaAndTableName, int mergeAmount, params object[] content)
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

            if (hexColor == null)
                return null;

            return ColorTranslator.FromHtml(hexColor);
        }

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

            if (!Context.DocumenterSettings.NoInternalDataTypes)
                WriteLine(table.SchemaAndTableName, "Column Name", "Data Type (DbTools)", "Data Type", "Column Length", "Column Scale", "Allow Nulls", "Primary Key", "Identity", "Default Value", "Description", "Foreign Key Name", "Referenced Table", "Link", "Referenced Column");
            else
                WriteLine(table.SchemaAndTableName, "Column Name", "Data Type", "Column Length", "Column Scale", "Allow Nulls", "Primary Key", "Identity", "Default Value", "Description", "Foreign Key Name", "Referenced Table", "Link", "Referenced Column");
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
            return list.Where(x => !ShouldSkipKnownTechnicalTable(x.SchemaAndTableName)).ToList();
        }

        protected void AddForeignKey(ForeignKey fk, string firstColumn = null)
        {
            var countToMerge = 0;
            var table = fk.SqlTable;

            foreach (var fkColumn in fk.ForeignKeyColumns)
            {
                if (firstColumn != null)
                    Write(table.SchemaAndTableName, firstColumn);

                Write(table.SchemaAndTableName, fk.Name, fkColumn.ForeignKeyColumn.Name, Helper.GetSimplifiedSchemaAndTableName(fk.ReferredTable.SchemaAndTableName));
                WriteLink(table.SchemaAndTableName, "link", Helper.GetSimplifiedSchemaAndTableName(fk.ReferredTable.SchemaAndTableName), GetColor(fk.ReferredTable.SchemaAndTableName));
                Write(table.SchemaAndTableName, fkColumn.ReferredColumn.Name);

                if (fk.SqlEngineVersionSpecificProperties.Any())
                {
                    var propertySb = new StringBuilder();
                    foreach (var sqlEngineVersionSpecificProperty in fk.SqlEngineVersionSpecificProperties)
                    {
                        propertySb.Append(sqlEngineVersionSpecificProperty.Version)
                            .Append('/')
                            .Append(sqlEngineVersionSpecificProperty.Name)
                            .Append(" = ")
                            .AppendLine(sqlEngineVersionSpecificProperty.Value);
                    }

                    WriteLine(table.SchemaAndTableName, propertySb.ToString());
                }
                else
                {
                    WriteLine(table.SchemaAndTableName);
                }

                countToMerge++;
            }

            if (countToMerge > 1)
            {
                MergeUpFromPreviousRow(table.SchemaAndTableName, countToMerge - 1);
            }
        }

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

        protected void AddUniqueConstraint(UniqueConstraint uniqueConstraint, string firstColumn = null)
        {
            var countToMerge = 0;
            var table = uniqueConstraint.SqlTable;

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

        private static object[] FormatBoolContent(params object[] content)
        {
            var result = new List<object>();
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
            public string Description { get; set; }
            public bool IsPk { get; set; }
            public Identity Identity { get; set; }
            public DefaultValue DefaultValue { get; set; }
        }
    }
}
