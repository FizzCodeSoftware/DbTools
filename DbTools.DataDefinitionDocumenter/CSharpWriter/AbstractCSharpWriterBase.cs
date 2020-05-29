namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System;
    using System.Linq;
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
    }
}