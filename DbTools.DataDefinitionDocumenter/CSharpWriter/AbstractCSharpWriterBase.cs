﻿namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;

    public abstract class AbstractCSharpWriterBase
    {
        public GeneratorContext GeneratorContext { get; }
        public SqlEngineVersion Version { get; }
        public Type TypeMapperType { get; }

        protected AbstractCSharpWriterBase(GeneratorContext context, SqlEngineVersion version, Type typeMapperType)
        {
            GeneratorContext = context;
            Version = version;
            TypeMapperType = typeMapperType;
        }

        public abstract string GetColumnCreation(SqlColumn column, DocumenterHelper helper, string extraAnnotation, string comment);

        public virtual string GetSqlTypeNamespace()
        {
            return $"FizzCode.DbTools.DataDefinition.{Version}";
        }

        protected bool IsForeignKeyReferencedTableSkipped(SqlColumn column)
        {
            var fkOnColumn = column.Table.Properties.OfType<ForeignKey>().FirstOrDefault(fk => fk.ForeignKeyColumns.Any(fkc => fkc.ForeignKeyColumn == column));

            if (fkOnColumn == null)
                return false;

            return GeneratorContext.Customizer.ShouldSkip(fkOnColumn.ReferredTable.SchemaAndTableName);
        }

        protected string IsNullable(SqlColumn column)
        {
            if (column.Types[Version].IsNullable)
                return ", true";

            return "";
        }

        protected void AddForeignKeySettingsSingleColumn(StringBuilder sb, DocumenterHelper helper, ForeignKey fkOnColumn)
        {
            sb.Append(".SetForeignKeyToColumn(nameof(")
               // TODO spec name
               .Append(helper.GetSimplifiedSchemaAndTableName(fkOnColumn.ReferredTable.SchemaAndTableName, DatabaseDeclaration.SchemaTableNameSeparator.ToString(CultureInfo.InvariantCulture)))
               .Append("), \"")
               .Append(fkOnColumn.ForeignKeyColumns[0].ReferredColumn.Name)
               .Append("\"");

            // table.AddInt("PrimaryId").SetForeignKeyToTable(nameof(Primary), new SqlEngineVersionSpecificProperty(MsSqlVersion.MsSql2016, "Nocheck", "true")
            sb.Append(AddSqlEngineVersionSpecificProperties(fkOnColumn.SqlEngineVersionSpecificProperties));

            sb.Append(")");
        }

        protected abstract void AddForeignKeySettingsMultiColumn(StringBuilder sb, DocumenterHelper helper, ForeignKey fkOnColumn);

        private static string AddSqlEngineVersionSpecificProperties(SqlEngineVersionSpecificProperties sqlEngineVersionSpecificProperties)
        {
            // new[] {
            //    new SqlEngineVersionSpecificProperty(MsSqlVersion.MsSql2016, "Nocheck", "true"),
            //    new SqlEngineVersionSpecificProperty(MsSqlVersion.MsSql2016, "Nocheck", "true")
            // }

            var sb = new StringBuilder();

            if (sqlEngineVersionSpecificProperties.Any())
            {
                sb.Append(", ");
                if (sqlEngineVersionSpecificProperties.Count() == 1)
                {
                    var sqlEngineVersionSpecificProperty = sqlEngineVersionSpecificProperties.First();
                    sb.Append(AddSqlEngineVersionSpecificProperty(sqlEngineVersionSpecificProperty));
                }
                else
                {
                    sb.Append("new[] {");
                    foreach (var sqlEngineVersionSpecificProperty in sqlEngineVersionSpecificProperties)
                    {
                        sb.Append(AddSqlEngineVersionSpecificProperty(sqlEngineVersionSpecificProperty));
                    }

                    sb.Append("}");
                }
            }

            return sb.ToString();
        }

        private static string AddSqlEngineVersionSpecificProperty(SqlEngineVersionSpecificProperty sqlEngineVersionSpecificProperty)
        {
            var sb = new StringBuilder();

            sb.Append("new SqlEngineVersionSpecificProperty(")
                                .Append(sqlEngineVersionSpecificProperty.Version.GetType().Name)
                                .Append(".")
                                .Append(sqlEngineVersionSpecificProperty.Version)
                                .Append(", \"")
                                .Append(sqlEngineVersionSpecificProperty.Name)
                                .Append("\", \"")
                                .Append(sqlEngineVersionSpecificProperty.Value)
                                .Append("\")");

            return sb.ToString();
        }

        protected abstract string GetColumnCreationMethod(SqlColumn column);

        protected void AddForeignKeySettings(SqlColumn column, StringBuilder sb, DocumenterHelper helper)
        {
            var fkOnColumn = column.Table.Properties.OfType<ForeignKey>().FirstOrDefault(fk => fk.ForeignKeyColumns.Any(fkc => fkc.ForeignKeyColumn == column));
            if (fkOnColumn != null)
            {
                if (GeneratorContext.GeneratorSettings.ShouldCommentOutFkReferences
                    && GeneratorContext.Customizer?.ShouldSkip(fkOnColumn.ReferredTable.SchemaAndTableName) == true)
                {
                    sb.Append("; //");
                }

                // TODO gen AddForeignkeys?
                if (fkOnColumn.ForeignKeyColumns.Count == 1)
                {
                    AddForeignKeySettingsSingleColumn(sb, helper, fkOnColumn);
                }
                else
                {
                    // Only create after last
                    if (column == fkOnColumn.ForeignKeyColumns.Last().ForeignKeyColumn)
                    {
                        AddForeignKeySettingsMultiColumn(sb, helper, fkOnColumn);
                    }
                }
            }
        }
    }
}