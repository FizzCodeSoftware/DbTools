namespace FizzCode.DbTools.DataDefinition.SqLite3
{
    using System.Collections.Generic;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.SqlGenerator;

    public class SqLite3Generator : AbstractSqlGenerator
    {
        public SqLite3Generator(Context context)
            : base(context)
        {
            Version = SqLiteVersion.SqLite3;
        }

        public override string GuardKeywords(string name)
        {
            return $"\"{name}\"";
        }

        public override string DropAllForeignKeys()
        {
            throw new System.NotImplementedException();
        }

        public override string DropAllViews()
        {
            throw new System.NotImplementedException();
        }

        public override string DropAllTables()
        {
            throw new System.NotImplementedException();
        }

        public override SqlStatementWithParameters DropSchemas(List<string> schemaNames, bool hard = false)
        {
            throw new System.NotImplementedException();
        }

        public override SqlStatementWithParameters TableExists(SqlTable table)
        {
            throw new System.NotImplementedException();
        }

        public override string DropAllIndexes()
        {
            throw new System.NotImplementedException();
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
    }
}