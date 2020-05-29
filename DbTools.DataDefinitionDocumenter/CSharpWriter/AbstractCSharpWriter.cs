namespace FizzCode.DbTools.DataDefinitionDocumenter
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
    }

    public abstract class AbstractCSharpTypedWriter : AbstractCSharpWriterBase
    {
        protected AbstractCSharpTypedWriter(GeneratorContext context, SqlEngineVersion version, Type typeMapperType)
            : base(context, version, typeMapperType)
        {
        }

        public override string GetColumnCreation(SqlColumn column, DocumenterHelper helper, string extraAnnotation, string comment)
        {
            var sb = new StringBuilder();

            /*if (GeneratorContext.GeneratorSettings.ShouldCommentOutColumnsWithFkReferencedTables
                && IsForeignKeyReferencedTableSkipped(column))
            {
                sb.Append("// ");
            }

            sb.Append(3, "table.")
                .Append(GetColumnCreationMethod(column));

            sb.Append(IsNullable(column));

            sb.Append(")");

            if (column.Table.Properties.OfType<PrimaryKey>().Any(x => x.SqlColumns.Any(y => y.SqlColumn == column)))
            {
                sb.Append(".SetPK()");
            }

            if (column.Properties.OfType<Identity>().Any())
            {
                sb.Append(".SetIdentity()");
            }

            if (!GeneratorContext.GeneratorSettings.NoForeignKeys)
                AddForeignKeySettings(column, sb, helper);

            // TODO Default Value + config

            if (!string.IsNullOrEmpty(extraAnnotation))
            {
                sb.Append(extraAnnotation);
            }

            sb.Append(";");

            var descriptionProperty = column.Properties.OfType<SqlColumnDescription>().FirstOrDefault();
            var description = descriptionProperty?.Description
                ?.Replace("\r", "", StringComparison.OrdinalIgnoreCase)
                ?.Replace("\n", "", StringComparison.OrdinalIgnoreCase)
                ?.Trim();

            if (!string.IsNullOrEmpty(description))
            {
                sb.Append(" // ").Append(description);

                if (!string.IsNullOrEmpty(comment))
                {
                    sb.Append(" ").Append(comment);
                }
            }
            else if (!string.IsNullOrEmpty(comment))
            {
                sb.Append(" // ").Append(comment);
            }
            */
            return sb.ToString();
        }

        protected abstract string GetColumnCreationMethod(SqlColumn column);
    }

    public abstract class AbstractCSharpWriter : AbstractCSharpWriterBase
    {
        protected AbstractCSharpWriter(GeneratorContext context, SqlEngineVersion version, Type typeMapperType)
            : base(context, version, typeMapperType)
        {
        }

        public override string GetColumnCreation(SqlColumn column, DocumenterHelper helper, string extraAnnotation, string comment)
        {
            var sb = new StringBuilder();

            if (GeneratorContext.GeneratorSettings.ShouldCommentOutColumnsWithFkReferencedTables
                && IsForeignKeyReferencedTableSkipped(column))
            {
                sb.Append("// ");
            }

            sb.Append(3, "table.")
                .Append(GetColumnCreationMethod(column));

            sb.Append(IsNullable(column));

            sb.Append(")");

            if (column.Table.Properties.OfType<PrimaryKey>().Any(x => x.SqlColumns.Any(y => y.SqlColumn == column)))
            {
                sb.Append(".SetPK()");
            }

            if (column.Properties.OfType<Identity>().Any())
            {
                sb.Append(".SetIdentity()");
            }

            if (!GeneratorContext.GeneratorSettings.NoForeignKeys)
                AddForeignKeySettings(column, sb, helper);

            // TODO Default Value + config

            if (!string.IsNullOrEmpty(extraAnnotation))
            {
                sb.Append(extraAnnotation);
            }

            sb.Append(";");

            var descriptionProperty = column.Properties.OfType<SqlColumnDescription>().FirstOrDefault();
            var description = descriptionProperty?.Description
                ?.Replace("\r", "", StringComparison.OrdinalIgnoreCase)
                ?.Replace("\n", "", StringComparison.OrdinalIgnoreCase)
                ?.Trim();

            if (!string.IsNullOrEmpty(description))
            {
                sb.Append(" // ").Append(description);

                if (!string.IsNullOrEmpty(comment))
                {
                    sb.Append(" ").Append(comment);
                }
            }
            else if (!string.IsNullOrEmpty(comment))
            {
                sb.Append(" // ").Append(comment);
            }

            return sb.ToString();
        }

        private void AddForeignKeySettings(SqlColumn column, StringBuilder sb, DocumenterHelper helper)
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
                else
                {
                    // Only create after last
                    if (column == fkOnColumn.ForeignKeyColumns.Last().ForeignKeyColumn)
                    {
                        sb.AppendLine(";")
                            .Append(3, "table.SetForeignKeyTo(nameof(")
                            .Append(helper.GetSimplifiedSchemaAndTableName(fkOnColumn.ReferredTable.SchemaAndTableName, DatabaseDeclaration.SchemaTableNameSeparator.ToString(CultureInfo.InvariantCulture)))
                            .AppendLine("), ");

                        sb.Append("new List<ColumnReference>()")
                            .AppendLine(3, "{");

                        foreach (var fkColumnMap in fkOnColumn.ForeignKeyColumns)
                        {
                            sb.Append(4, "new ColumnReference(\"").Append(fkColumnMap.ForeignKeyColumn.Name).Append("\", ").Append(fkColumnMap.ReferredColumn.Name).AppendLine("\"),");
                        }

                        sb.Append(3, "})");
                    }
                }
            }
        }

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

        protected bool IsForeignKeyReferencedTableSkipped(SqlColumn column)
        {
            var fkOnColumn = column.Table.Properties.OfType<ForeignKey>().FirstOrDefault(fk => fk.ForeignKeyColumns.Any(fkc => fkc.ForeignKeyColumn == column));

            if (fkOnColumn == null)
                return false;

            return GeneratorContext.Customizer.ShouldSkip(fkOnColumn.ReferredTable.SchemaAndTableName);
        }

        protected abstract string GetColumnCreationMethod(SqlColumn column);

        protected string IsNullable(SqlColumn column)
        {
            if (column.Types[Version].IsNullable)
                return ", true";

            return "";
        }
    }
}