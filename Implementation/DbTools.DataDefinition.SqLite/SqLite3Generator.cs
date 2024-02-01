namespace FizzCode.DbTools.DataDefinition.SqLite3
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition.Base;
    using FizzCode.DbTools.DataDefinition.SqlGenerator;
    using FizzCode.DbTools.SqlGenerator.SqLite;

    public class SqLite3Generator : AbstractSqlGenerator
    {
        public SqLite3Generator(Context context)
            : base(new SqLiteGenerator(context))
        {
            SqlVersion = SqLiteVersion.SqLite3;
        }

        public override string DropAllForeignKeys()
        {
            throw new NotImplementedException();
        }

        public override string DropAllViews()
        {
            throw new NotImplementedException();
        }

        public override string DropAllTables()
        {
            throw new NotImplementedException();
        }

        public override SqlStatementWithParameters DropSchemas(List<string> schemaNames, bool hard = false)
        {
            throw new NotImplementedException();
        }

        public override SqlStatementWithParameters TableExists(SqlTable table)
        {
            throw new NotImplementedException();
        }

        public override string DropAllIndexes()
        {
            throw new NotImplementedException();
        }

        public override string CreateTable(SqlTable table)
        {
            return CreateTableInternal(table, true);
        }

        // SqLite does not support ALTER TABLE ... ADD CONSTRAINT
        public override string CreateForeignKey(ForeignKey fk)
        {
            return "";
        }

        protected override void GenerateCreateColumnIdentity(StringBuilder sb, Identity identity)
        {
            // TODO make a setting for use or omit of AUTOINCREMENT
            // see https://www.sqlite.org/autoinc.html

            var sqlTable = (SqlTable)identity.SqlColumn.SqlTableOrView;
            var pk = sqlTable.Properties.OfType<PrimaryKey>().FirstOrDefault();

            // TODO validate beforehand?
            // TODO give descriptive message including column names, identity and PK declarations

            if (!(pk == null && (bool)Context.Settings.SqlVersionSpecificSettings["ShouldCreateAutoincrementAsPrimaryKey"]))
            { 
                if (pk == null || pk.SqlColumns.Count == 0)
                    throw new InvalidOperationException("Identity (AUTOINCREMENT) is only supported with Primary Key.");
                else if (pk.SqlColumns.Count > 1)
                    throw new InvalidOperationException("Identity (AUTOINCREMENT) is only supported with the same single column as Primary Key.");
                else if (pk.SqlColumns[0].SqlColumn.Name != identity.SqlColumn.Name)
                    throw new InvalidOperationException("Identity (AUTOINCREMENT) is only supported with the same single column as Primary Key. The Primary Key is on a different column.");
            }

            sb.Append(" PRIMARY KEY AUTOINCREMENT");
        }

        protected override void CreateTablePrimaryKey(SqlTable table, StringBuilder sb)
        {
            // PRIMARY KEY with IDENTITY should only be declared with the Column,
            // not as a separate constraint
            var pk = table.Properties.OfType<PrimaryKey>().FirstOrDefault();

            if (pk != null
                && pk.SqlColumns.Any(pkc => pkc.SqlColumn.Properties.OfType<Identity>().FirstOrDefault() != null))
                return;

            base.CreateTablePrimaryKey(table, sb);
        }
    }
}