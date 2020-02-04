namespace FizzCode.DbTools.DataDefinitionGenerator
{
    using System.Collections.Generic;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;

    public class SqLiteGenerator3 : GenericSqlGenerator
    {
        public SqLiteGenerator3(Context context)
            : base(context)
        {
            Version = SqlVersions.SqLite3;
        }

        protected override string GuardKeywords(string name)
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
        public override string CreateForeignKeys(SqlTable table)
        {
            return "";
        }
    }
}